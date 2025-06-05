using ErrDLogiPTClient.Scene.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Button;

public class BasicButtonPlaySoundEventArgs : BasicButtonEventArgs
{
    // Fields.
    public BasicButtonSoundOrigin Origin { get; }
    public ILogiSoundInstance? Sound { get; set; }




    // Constructors.
    public BasicButtonPlaySoundEventArgs(IBasicButton button, BasicButtonSoundOrigin origin, ILogiSoundInstance sound)
        : base(button)
    {
        Origin = origin;
        Sound = sound ?? throw new ArgumentNullException(nameof(sound));
    }
}