using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.Checkmark;
using ErrDLogiPTClient.Scene.UI.Dropdown;
using ErrDLogiPTClient.Scene.UI.Slider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public interface IUIElementFactory
{
    void LoadAssets();
    IBasicButton CreateButton();
    IBasicCheckmark CreateCheckmark();
    IBasicDropdownList<T> CreateDropdownList<T>();
    IBasicSlider CreateSlider();
}