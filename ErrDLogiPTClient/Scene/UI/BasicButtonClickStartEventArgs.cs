using GHEngine.Audio.Source;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class BasicButtonClickStartEventArgs : BasicButtonEventArgs
{
    // Fields.
    public UIElementClickType ClickType { get; }
    public Vector2 ClickStartLocation { get; }


    // Constructors.
    public BasicButtonClickStartEventArgs(UIBasicButton button,
        UIElementClickType clickType,
        Vector2 startLocation) : base(button)
    {
        ClickType = clickType;
        ClickStartLocation = startLocation;
    }
}