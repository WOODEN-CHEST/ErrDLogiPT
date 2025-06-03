using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Button;

public abstract class BasicButtonEventArgs : CancellableEventBase
{
    // Fields.
    public IBasicButton Button { get; private init; }


    // Constructors.
    public BasicButtonEventArgs(IBasicButton button)
    {
        Button = button ?? throw new ArgumentNullException(nameof(button));
    }
}