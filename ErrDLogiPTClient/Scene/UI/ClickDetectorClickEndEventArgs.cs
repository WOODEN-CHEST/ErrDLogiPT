using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class ClickDetectorClickEndEventArgs : ClickDetectorEventArgs
{
    // Fields
    public Vector2 ClickStartLocation { get; }
    public Vector2 ClickEndLocation { get; }
    public UIElementClickType ClickType { get; }
    public TimeSpan ClickDuration { get; }
    public bool WasClickedInBounds { get; }


    // Constructors.
    public ClickDetectorClickEndEventArgs(ClickDetector detector,
        Vector2 startLocation,
        Vector2 endLocation,
        UIElementClickType clickType,
        TimeSpan clickDuration,
        bool wasClickedInBounds) : base(detector)
    {
        ClickStartLocation = startLocation;
        ClickEndLocation = endLocation;
        ClickType = clickType;
        ClickDuration = clickDuration;
        WasClickedInBounds = wasClickedInBounds;
    }
}