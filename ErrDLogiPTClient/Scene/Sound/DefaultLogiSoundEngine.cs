using GHEngine;
using GHEngine.Audio;
using GHEngine.Audio.Source;
using NAudio.Wave;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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


    // Inherited methods.
    public virtual ILogiSoundInstance CreateSoundInstance(IPreSampledSound sound, LogiSoundCategory category)
    {
        ArgumentNullException.ThrowIfNull(sound, nameof(sound));
        ArgumentNullException.ThrowIfNull(category, nameof(category));

        ILogiSoundInstance Instance = new DefaultSceneSoundInstance((IPreSampledSoundInstance)sound.CreateInstance(), category);
        _soundsToAdd.Add(Instance);
        Instance.SoundDataUpdate += OnSoundDataUpdateEvent;
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

        _soundsToUpdate.Clear();
        _soundsToAdd.Clear();
        _soundsToRemove.Clear();

        _wrappedEngine.ScheduleAction(() => 
        {
            SyncSoundsToAudioEngine(SoundsToUpdate, SoundsToAdd, SoundsToRemove);
        });
    }

    public void ScheduleAction(Action action)
    {
        _scheduledActions.Enqueue(action ?? throw new ArgumentNullException(nameof(action)));
    }
}