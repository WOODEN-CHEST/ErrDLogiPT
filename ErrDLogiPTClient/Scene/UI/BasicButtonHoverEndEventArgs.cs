﻿using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class BasicButtonHoverEndEventArgs : BasicButtonEventArgs
{
    // Fields.
    public IPreSampledSound? Sound { get; set; }


    // Constructors.
    public BasicButtonHoverEndEventArgs(UIBasicButton button) : base(button)
    {
    }
}