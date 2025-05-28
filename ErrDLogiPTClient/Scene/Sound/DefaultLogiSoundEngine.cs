using GHEngine;
using GHEngine.Audio;
using GHEngine.Audio.Source;
using NAudio.Wave;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public class DefaultLogiSoundEngine : ILogiSoundEngine
{
    // Static fields.
    public const float VOLUME_MIN = 0f;
    public const float VOLUME_DEFAULT = 1f;
    public const float VOLUME_MAX = 100f;


    // Fields.
    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid master volume: {value}", nameof(value));
            }
            if (_masterVolume == value)
            {
                return;
            }

            _shouldMasterVolumeUpdate = true;
            _masterVolume = Math.Clamp(value, VOLUME_MIN, VOLUME_MAX);
        }
    }

    public int SoundCount => _sounds.Count;
    public WaveFormat Format => _wrappedEngine.WaveFormat;
    public IEnumerable<ILogiSoundInstance> Sounds => _sounds.Values;


    // Private fields.
    private readonly Dictionary<LogiSoundCategory, float> _categoryVolumes = new();
    private readonly Dictionary<ISoundInstance, ILogiSoundInstance> _sounds = new();
    private readonly HashSet<ILogiSoundInstance> _soundsToRemove = new();
    private readonly HashSet<ILogiSoundInstance> _soundsToAdd = new();
    private readonly HashSet<ILogiSoundInstance> _soundsToUpdate = new();
    private readonly IAudioEngine _wrappedEngine;
    private readonly ConcurrentQueue<Action> _scheduledActions = new();
    private readonly HashSet<LogiSoundCategory> _categoriesToUpdate = new();
    private readonly SoundInstanceSynchronizer _soundSynchronizer = new();

    private float _masterVolume = 1f;
    private bool _shouldMasterVolumeUpdate = false;


    // Constructors.
    public DefaultLogiSoundEngine(IAudioEngine engine)
    {
        _wrappedEngine = engine ?? throw new ArgumentNullException(nameof(engine));
    }


    // Private methods.
    private void ExecuteScheduledActions()
    {
        while (_scheduledActions.TryDequeue(out Action? TargetAction))
        {
            TargetAction.Invoke();
        }
    }

    private void OnSoundDataUpdateEvent(object? sender, LogiSoundEventArgs args)
    {
        _soundsToUpdate.Add(args.SoundInstance);
    }

    private void OnSoundLoopEvent(object? sender, SoundLoopedArgs args)
    {
        ScheduleAction(() => 
        {
            if (_sounds.TryGetValue(args.Sound, out ILogiSoundInstance? LogiSound))
            {
                LogiSound.InvokeLoopEvent();
            }
        });
    }

    private void OnSoundFinishEvent(object? sender, SoundFinishedArgs args)
    {
        ScheduleAction(() =>
        {
            if (_sounds.TryGetValue(args.Instance, out ILogiSoundInstance? LogiSound))
            {
                LogiSound.InvokeFinishEvent();
            }
        });
    }

    private ILogiSoundInstance[] GetSoundSnapshot(HashSet<ILogiSoundInstance> sounds)
    {
        if (sounds.Count == 0)
        {
            return Array.Empty<ILogiSoundInstance>();
        }
        return sounds.ToArray();
    }

    private void SyncSoundsToAudioEngine(ILogiSoundInstance[] soundsToUpdate,
        ILogiSoundInstance[] soundsToAdd,
        ILogiSoundInstance[] soundsToRemove)
    {
        foreach (ILogiSoundInstance UpdatedSound in soundsToUpdate)
        {
            _soundSynchronizer.SynchronzieSound(UpdatedSound);
        }
        foreach (ILogiSoundInstance AddedSound in soundsToAdd)
        {
            _wrappedEngine.AddSoundInstance(AddedSound.WrappedSoundInstance);
        }
        foreach (ILogiSoundInstance RemovedSound in soundsToRemove)
        {
            _wrappedEngine.RemoveSoundInstance(RemovedSound.WrappedSoundInstance);
        }

        if (_wrappedEngine.SoundCount == 0)
        {
            return;
        }

        SyncSoundsBackToWrapperEngine();
    }

    private void SyncSoundsBackToWrapperEngine()
    {
        foreach (ISoundInstance Instance in _wrappedEngine.Sounds)
        {
            if (Instance is not IPreSampledSoundInstance PreSampleInstance)
            {
                continue;
            }

            TimeSpan Position = PreSampleInstance.Position;
            ScheduleAction(() =>
            {
                if (!_sounds.TryGetValue(Instance, out ILogiSoundInstance? LogiInstance))
                {
                    return;
                }
                LogiInstance.Position = Position;
            });
        }
    }

    private CategoryVolumeUpdateData[] GetCategoryUpdateDataSnapshot()
    {
        if (_categoriesToUpdate.Count == 0)
        {
            return Array.Empty<CategoryVolumeUpdateData>();
        }

        CategoryVolumeUpdateData[] CategoryDataArray = new CategoryVolumeUpdateData[_categoriesToUpdate.Count];

        int Index = 0;
        foreach (LogiSoundCategory Category in _categoriesToUpdate)
        {
            SoundVolumeData[] VolumeData = _sounds.Values
                .Where(sound => sound.Category == Category)
                .Select(sound => new SoundVolumeData(sound.WrappedSoundInstance, sound.Volume))
                .ToArray();

            CategoryDataArray[Index] = new(Category, GetCategoryVolume(Category), VolumeData);
            Index++;
        }

        return CategoryDataArray;
    }

    private void SyncCategoriesToUpdate(CategoryVolumeUpdateData[] categoriesToUpdate)
    {
        foreach (CategoryVolumeUpdateData CategoryData in categoriesToUpdate)
        {
            foreach (SoundVolumeData VolumeData in CategoryData.Sounds)
            {
                VolumeData.WrappedSound.Sampler.Volume = VolumeData.Volume * CategoryData.Volume;
            }
        }
    }


    // Inherited methods.
    public virtual ILogiSoundInstance CreateSoundInstance(IPreSampledSound sound, LogiSoundCategory category)
    {
        ArgumentNullException.ThrowIfNull(sound, nameof(sound));
        ArgumentNullException.ThrowIfNull(category, nameof(category));

        ILogiSoundInstance Instance = new DefaultSceneSoundInstance((IPreSampledSoundInstance)sound.CreateInstance(), category);
        _soundsToAdd.Add(Instance);

        Instance.WrappedSoundInstance.SoundLooped += OnSoundLoopEvent;
        Instance.WrappedSoundInstance.SoundFinished += OnSoundFinishEvent;
        Instance.SoundDataUpdate += OnSoundDataUpdateEvent;

        if (!_categoryVolumes.ContainsKey(category))
        {
            SetCategoryVolume(category, VOLUME_DEFAULT);
        }

        return Instance;
    }

    public virtual float GetCategoryVolume(LogiSoundCategory category)
    {
        ArgumentNullException.ThrowIfNull(category, nameof(category));
        return _categoryVolumes.GetValueOrDefault(category, 0f);
    }

    public virtual void SetCategoryVolume(LogiSoundCategory category, float volume)
    {
        ArgumentNullException.ThrowIfNull(category, nameof(category));
        if (float.IsNormal(volume) || float.IsInfinity(volume))
        {
            throw new ArgumentException($"Invalid category volume: {volume}");
        }

        _categoryVolumes[category] = Math.Clamp(volume, VOLUME_MIN, VOLUME_MAX);

        foreach (ILogiSoundInstance SoundInstance in _sounds.Values.Where(sound => sound.Category == category))
        {
            _soundsToUpdate.Add(SoundInstance);
        }

        _categoriesToUpdate.Add(category);
    }

    public virtual void RemoveAllSounds()
    {
        foreach (ILogiSoundInstance Instance in _sounds.Values.ToArray())
        {
            RemoveSound(Instance);
        }
    }

    public virtual void RemoveSounds(LogiSoundCategory category)
    {
        ArgumentNullException.ThrowIfNull(category, nameof(category));
        foreach (ILogiSoundInstance Instance in _sounds.Values.Where(sound => sound.Category == category).ToArray())
        {
            RemoveSound(Instance);
        }
    }

    public void RemoveSounds(IPreSampledSound sound)
    {
        ArgumentNullException.ThrowIfNull(sound, nameof(sound));
        foreach (ILogiSoundInstance Instance in _sounds.Values.Where(sound => sound.WrappedSoundInstance == sound).ToArray())
        {
            RemoveSound(Instance);
        }
    }

    public void RemoveSound(ILogiSoundInstance soundInstance)
    {
        ArgumentNullException.ThrowIfNull(soundInstance, nameof(soundInstance));
        if (_sounds.Remove(soundInstance.WrappedSoundInstance))
        {
            _soundsToRemove.Add(soundInstance);
            soundInstance.SoundDataUpdate -= OnSoundDataUpdateEvent;
            soundInstance.WrappedSoundInstance.SoundLooped -= OnSoundLoopEvent;
            soundInstance.WrappedSoundInstance.SoundFinished -= OnSoundFinishEvent;
        }
    }

    public void Start()
    {
        _wrappedEngine.Start();
    }

    public void Stop()
    {
        _wrappedEngine.Stop();
    }

    public void Dispose()
    {
        _wrappedEngine.Dispose();
    }

    public virtual void Update(IProgramTime time)
    {
        ExecuteScheduledActions();

        ILogiSoundInstance[] SoundsToUpdate = GetSoundSnapshot(_soundsToUpdate);
        ILogiSoundInstance[] SoundsToAdd = GetSoundSnapshot(_soundsToAdd);
        ILogiSoundInstance[] SoundsToRemove = GetSoundSnapshot(_soundsToRemove);
        CategoryVolumeUpdateData[] CategoryVolumeData = GetCategoryUpdateDataSnapshot();

        _soundsToUpdate.Clear();
        _soundsToAdd.Clear();
        _soundsToRemove.Clear();
        _categoriesToUpdate.Clear();

        float NewVolume = MasterVolume;
        bool UpdateMasterVolume = _shouldMasterVolumeUpdate;
        _shouldMasterVolumeUpdate = false;

        _wrappedEngine.ScheduleAction(() => 
        {
            SyncSoundsToAudioEngine(SoundsToUpdate, SoundsToAdd, SoundsToRemove);
            SyncCategoriesToUpdate(CategoryVolumeData);
            if (UpdateMasterVolume)
            {
                _wrappedEngine.Volume = NewVolume;
            }
        });
    }

    public void ScheduleAction(Action action)
    {
        _scheduledActions.Enqueue(action ?? throw new ArgumentNullException(nameof(action)));
    }


    // Types.
    private record class SoundVolumeData(IPreSampledSoundInstance WrappedSound, float Volume);
    private record class CategoryVolumeUpdateData(LogiSoundCategory Category, float Volume, SoundVolumeData[] Sounds);
}