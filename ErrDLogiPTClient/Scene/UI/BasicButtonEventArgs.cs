using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public abstract class BasicButtonEventArgs : CancellableEventBase
{
    // Fields.
    public UIBasicButton Button { get; private init; }


    // Constructors.
    public BasicButtonEventArgs(UIBasicButton button)
    {
        Button = button ?? throw new ArgumentNullException(nameof(button));
    }
}