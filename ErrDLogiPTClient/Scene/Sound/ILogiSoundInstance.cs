using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Audio;

namespace ErrDLogiPTClient.Scene.Sound;

public interface ILogiSoundInstance
{
    // Fields.
    LogiSoundCategory Category { get; set; }
    SceneSoundState State { get; set; }
    bool IsUpdateRequired { get; }
    TimeSpan Duration { get; }
    float Volume { get; set; }
    float? CustomSampleRate { get; set; }
    double Speed { get; set; }
    float? LowPassFrequency { get; set; }
    float? HighPassFrequency { get; set; }
    float Pan { get; set; }
    TimeSpan Position { get; set; }
    bool IsPositionChangeRequested { get; }
    IPreSampledSoundInstance WrappedSoundInstance { get; }
    bool IsLooped { get; set; }
    event EventHandler<LogiSoundEventArgs>? SoundFinish;
    event EventHandler<LogiSoundEventArgs>? SoundLoop;
    event EventHandler<LogiSoundEventArgs>? SoundDataUpdate;


    // Methods.
    void SyncWrappedProperties(TimeSpan newPosition);
    void InvokeLoopEvent();
    void InvokeFinishEvent();
}