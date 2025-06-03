using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Button;

public abstract class BasicButtonMainHandlerArgs
{
    // Fields.
    public IBasicButton Button { get; }


    // Constructors.
    public BasicButtonMainHandlerArgs(IBasicButton button)
    {
        Button = button ?? throw new ArgumentNullException(nameof(button));
    }
}