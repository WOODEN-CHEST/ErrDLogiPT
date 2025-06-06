using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

/// <summary>
/// The method used by various UI elements to determine whether they was interacted with.
/// </summary>
public enum ElementClickMethod
{
    /// <summary>
    /// The element should activate immediately when the mouse button is pressed down while in bounds of the button.
    /// </summary>
    ActivateOnClick,

    /// <summary>
    /// The element should activate immediately when the mouse button is released while in bounds of the button.
    /// The starting position of the mouse cursor when the mouse button was first pressed down doesn't matter.
    /// </summary>
    ActivateOnRelease,

    /// <summary>
    /// The element should activate only if the mouse button was both pressed down and released while in bounds
    /// of the button.
    /// </summary>
    ActivateOnFullClick
}