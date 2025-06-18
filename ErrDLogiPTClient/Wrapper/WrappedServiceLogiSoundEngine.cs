using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Service;
using GHEngine;
using GHEngine.Audio.Source;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrappedServiceLogiSoundEngine : ServiceWrapper<ILogiSoundEngine>, ILogiSoundEngine
{
    // Fields.
    public float MasterVolume
    {
        get => ServiceObject.MasterVolume;
        set => ServiceObject.MasterVolume = value;
    }
    public int SoundCount => ServiceObject.SoundCount;
    public IEnumerable<ILogiSoundInstance> Sounds => ServiceObject.Sounds;
    public WaveFormat Format => ServiceObject.Format;


    // Constructors.
    public WrappedServiceLogiSoundEngine(IGenericServices services) : base(services) { }


    // Inherited methods.
    public void AddSoundInstance(ILogiSoundInstance soundInstance)
    {
        ServiceObject.AddSoundInstance(soundInstance);
    }

    public ILogiSoundInstance CreateSoundInstance(IPreSampledSound sound, LogiSoundCategory category)
    {
        return ServiceObject.CreateSoundInstance(sound, category);
    }

    public void Dispose() { }

    public float GetCategoryVolume(LogiSoundCategory category)
    {
        return ServiceObject.GetCategoryVolume(category);
    }

    public void RemoveAllSounds()
    {
        ServiceObject.RemoveAllSounds();
    }

    public void RemoveSound(ILogiSoundInstance soundInstance)
    {
        ServiceObject.RemoveSound(soundInstance);
    }

    public void RemoveSounds(LogiSoundCategory category)
    {
        ServiceObject.RemoveSounds(category);
    }

    public void RemoveSounds(IPreSampledSound sound)
    {
        ServiceObject.RemoveSounds(sound);
    }

    public void ScheduleAction(Action action)
    {
        ServiceObject.ScheduleAction(action);
    }

    public void SetCategoryVolume(LogiSoundCategory category, float volume)
    {
        ServiceObject.SetCategoryVolume(category, volume);
    }

    public void Start() { }

    public void Stop() { }

    public void Update(IProgramTime time) { }
}