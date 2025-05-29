using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public record class SoundPropertySnapshot(
    LogiSoundState State,
    bool IsLooped,
    float Volume,
    float Pan,
    float? LowPassFrequency,
    float? HighPassFrequency,
    double Speed,
    float? CustomSampleRate,
    TimeSpan? NewPosition)
{
    // Static methods.
    public static SoundPropertySnapshot Create(ILogiSoundInstance sound)
    {
        return new(sound.State,
            sound.IsLooped,
            sound.Volume,
            sound.Pan,
            sound.LowPassFrequency,
            sound.HighPassFrequency,
            sound.Speed,
            sound.CustomSampleRate,
            sound.IsPositionChangeRequested ? sound.Position : null);
    }
}