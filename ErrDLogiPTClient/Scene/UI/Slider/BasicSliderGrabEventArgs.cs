﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Slider;

public class BasicSliderGrabEventArgs : BasicSliderEventArgs
{
    public BasicSliderGrabEventArgs(IBasicSlider slider) : base(slider)
    {
    }
}
