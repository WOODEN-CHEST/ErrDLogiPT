using GHEngine.Audio.Source;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class BasicButtonClickEndEventArgs : BasicButtonEventArgs
{
    // Fields.
    public UIElementClickType ClickType { get; set; }
    public Vector2 ClickStartLocation { get; set; }
    public Vector2 ClickEndLocation { get; set; }
    public TimeSpan ClickDuration { get; set; }
    public IPreSampledSound? ClickSound { get; set; } = null;


    // Constructors.
    public BasicButtonClickEndEventArgs(UIBasicButton button,
        UIElementClickType clickType,
        Vector2 startLocation,
        Vector2 endLocation,
        TimeSpan clickDuration) : base(button)
    {
        ClickType = clickType;
        ClickStartLocation = startLocation;
        ClickEndLocation = endLocation;
        ClickDuration = clickDuration;
    }
}