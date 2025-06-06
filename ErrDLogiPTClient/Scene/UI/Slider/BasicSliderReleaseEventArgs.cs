using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Slider;

public class BasicSliderReleaseEventArgs : BasicSliderEventArgs
{
    public BasicSliderReleaseEventArgs(IBasicSlider slider) : base(slider)
    {
    }
}
