using GHEngine;
using GHEngine.Audio.Source;
using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public interface ILogiSoundEngine : ITimeUpdatable, IDisposable
{
    // Fields.
    float MasterVolume { get; set; }
    int SoundCount { get; }
    IEnumerable<ILogiSoundInstance> Sounds { get; }
    WaveFormat Format { get; }


    // Methods.
    void RemoveAllSounds();
    void RemoveSounds(LogiSoundCategory category);
    void RemoveSounds(IPreSampledSound sound);
    void RemoveSound(ILogiSoundInstance soundInstance);
    ILogiSoundInstance CreateSoundInstance(IPreSampledSound sound, LogiSoundCategory category);
    void AddSoundInstance(ILogiSoundInstance soundInstance);
    void SetCategoryVolume(LogiSoundCategory category, float volume);
    float GetCategoryVolume(LogiSoundCategory category);
    void ScheduleAction(Action action);
    void Start();
    void Stop();
}