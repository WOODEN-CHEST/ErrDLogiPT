using GHEngine.Audio.Source;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Button;

public class BasicButtonMainClickArgs : BasicButtonMainHandlerArgs
{
    // Fields.
    public UIElementClickType ClickType { get; }
    public Vector2 ClickStartLocation { get; }
    public Vector2 ClickEndLocation { get; }
    public TimeSpan ClickDuration { get; }


    // Constructors.
    public BasicButtonMainClickArgs(IBasicButton button,
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