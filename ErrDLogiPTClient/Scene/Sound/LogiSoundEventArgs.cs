using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public class LogiSoundEventArgs : EventArgs
{
    // Fields.
    public ILogiSoundInstance SoundInstance { get; }


    // Constructors.
    public LogiSoundEventArgs(ILogiSoundInstance soundInstance)
    {
        SoundInstance = soundInstance ?? throw new ArgumentNullException(nameof(soundInstance));
    }
}