using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.Checkmark;
using ErrDLogiPTClient.Scene.UI.Dropdown;
using ErrDLogiPTClient.Scene.UI.Slider;
using ErrDLogiPTClient.Scene.UI.UITextBox;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public interface IUIElementFactory
{
    // Fields.
    Color NormalColor { get; }
    Color HoverColor { get; }
    Color ClickColor { get; }
    Color ListElementColor { get; }
    Color ListElementSelectedColor { get; }
    Color UnavailableColor { get; }
    Color CheckmarkColor { get; }
    Color TextboxTextColor { get; }


    // Methods.
    void LoadAssets();
    IBasicButton CreateButton();
    IBasicCheckmark CreateCheckmark();
    IBasicDropdownList<T> CreateDropdownList<T>();
    IBasicSlider CreateSlider();
    IBasicTextBox CreateTextBox();
}