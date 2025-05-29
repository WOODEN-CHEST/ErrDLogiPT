using ErrDLogiPTClient.Scene.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class BasicButtonSoundEventArgs : BasicButtonEventArgs
{
    // Fields.
    public UISoundOrigin Origin { get; }
    public ILogiSoundInstance? Sound { get; set; }




    // Constructors.
    public BasicButtonSoundEventArgs(UIBasicButton button, UISoundOrigin origin, ILogiSoundInstance sound) : base(button)
    {
        Origin = origin;
        Sound = sound ?? throw new ArgumentNullException(nameof(sound));
    }
}