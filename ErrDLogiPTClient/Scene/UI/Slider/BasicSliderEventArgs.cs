using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Slider;

public class BasicSliderEventArgs : CancellableEventBase
{
    // Fields.
    public IBasicSlider Slider { get; private init; }


    // Constructors.
    public BasicSliderEventArgs(IBasicSlider slider)
    {
        Slider = slider ?? throw new ArgumentNullException(nameof(slider));
    }
}