﻿using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public interface IFulLScreenToggler : ITimeUpdatable
{
    // Fields.
    bool CanSwitchFullScreen { get; set; }


    // Methods.
    void RestoreFullScreenSize();
}