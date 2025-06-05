using ErrDLogiPTClient.Scene.Sound;
using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Checkmark;

public class BasicCheckmarkPlaySoundEventArgs : BasicCheckmarkEventArgs
{
    // Fields.
    public ILogiSoundInstance? Sound { get; set; }
    public BasicCheckmarkSoundOrigin Origin { get; }


    // Constructors.
    public BasicCheckmarkPlaySoundEventArgs(IBasicCheckmark checkmark,
        ILogiSoundInstance sound,
        BasicCheckmarkSoundOrigin origin) : base(checkmark)
    {
        Sound = sound;
        Origin = origin;
    }
}