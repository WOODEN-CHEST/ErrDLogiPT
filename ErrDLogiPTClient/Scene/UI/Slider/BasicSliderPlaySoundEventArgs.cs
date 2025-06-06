using ErrDLogiPTClient.Scene.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Slider;

public class BasicSliderPlaySoundEventArgs : BasicSliderEventArgs
{
    // Fields.
    public ILogiSoundInstance? Sound { get; set; }
    

    // Constructors.
    public BasicSliderPlaySoundEventArgs(IBasicSlider slider, ILogiSoundInstance sound) : base(slider)
    {
        Sound = sound;
    }
}