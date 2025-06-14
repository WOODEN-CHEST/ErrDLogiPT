using GHEngine;
using GHEngine.Frame;
using GHEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.UITextBox;

public interface IBasicTextBox : IUIElement
{
    // Fields.
    Vector2 Position { get; set; }
    bool IsTypingEnabled { get; set; }
    float Scale {  get; set; }
    Vector2 Dimensions { get; set; }
    RectangleF Bounds { get; }
    Color GlobalTextColor { get; set; }
    float GlobalTextBrightness { get; set; }
    float GlobalTextOpacity { get; set; }
    Color BoxColor { get; set; }
    float ScrollFactor { get; set; }
    float ScrollFactorMin { get; }
    float ScrollFactorMax { get; }
    public string Text { get; set; }
    public Color TextColor { get; set; }
    TextAlignOption Alignment { get; set; }



    // Methods.
    bool IsPositionOverArea(Vector2 position);
    void PrepareTexturesForRendering();
}