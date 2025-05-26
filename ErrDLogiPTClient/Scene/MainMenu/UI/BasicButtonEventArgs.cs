using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu.UI;

public class BasicButtonEventArgs : CancellableEventBase
{
    // Fields.
    public MainMenuBasicButton Button { get; private init; }


    // Constructors.
    public BasicButtonEventArgs(MainMenuBasicButton button)
    {
        Button = button ?? throw new ArgumentNullException(nameof(button));
    }
}