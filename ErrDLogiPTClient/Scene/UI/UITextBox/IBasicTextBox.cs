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
    bool IsTextShadowEnabled { get; set; }
    float ShadowBrightness { get; set; }
    Vector2 ShadowOffset { get; set; }
    bool IsTypingEnabled { get; set; }
    float Scale {  get; set; }
    Vector2 Dimensions { get; set; }
    RectangleF Bounds { get; }
    Color GlobalTextColor { get; set; }
    float GlobalTextBrightness { get; set; }
    float GlobalTextOpacity { get; set; }
    Color BoxColor { get; set; }

    int ComponentCount { get; }
    IEnumerable<TextComponent> Components { get; }



    // Methods.
    bool IsPositionOverArea(Vector2 position);
    void AddComponent(TextComponent component);
    void RemoveComponent(TextComponent component);
    void InsertComponent(TextComponent component, int index);
    void ClearComponents(TextComponent component);
    void PrepareTexturesForRendering();
}