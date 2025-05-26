using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public interface ISceneSoundInstance
{
    // Fields.
    bool IsUpdateRequired { get; }
    TimeSpan Duration { get; }
    float Volume { get; set; }
    float? CustomSampleRate { get; set; }
    double Speed { get; set; }
    float? LowPassFrequency { get; set; }
    float? HighPassFrequency { get; set; }
    float Pan { get; set; }
    TimeSpan Position { get; set; }
    IPreSampledSoundInstance WrappedSoundInstance { get; }


    // Methods.
    void Start();
    void Stop();
    void Continue();
    void SyncProperties();
}