using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public interface IMainMenuUISection
{
    // Fields.
    bool IsEnabled { get; set; }
    bool IsVisible { get; set; }
}