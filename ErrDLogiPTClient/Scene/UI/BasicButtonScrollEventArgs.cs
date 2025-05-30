﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class BasicButtonScrollEventArgs : BasicButtonEventArgs
{
    // Fields.
    public int ScrollAmount { get; set; }


    // Constructors.
    public BasicButtonScrollEventArgs(UIBasicButton button, int scrollAmount) : base(button)
    {
        ScrollAmount = scrollAmount;
    }
}