using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class ClickDetectorClickStartEventArgs : ClickDetectorEventArgs
{
    // Fields.
    public Vector2 ClickPosition { get; }
    public UIElementClickType ClickType { get; }


    // Constructors.
    public ClickDetectorClickStartEventArgs(ClickDetector detector, 
        Vector2 clickPosition, 
        UIElementClickType clickType) : base(detector)
    {
        ClickPosition = clickPosition;
        ClickType = clickType;
    }
}