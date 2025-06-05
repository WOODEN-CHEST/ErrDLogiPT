using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Checkmark;

public class BasicCheckmarkEventArgs : CancellableEventBase
{
    // Fields.
    public IBasicCheckmark Checkmark { get; private init; }


    // Constructors.
    public BasicCheckmarkEventArgs(IBasicCheckmark checkmark)
    {
        Checkmark = checkmark ?? throw new ArgumentNullException(nameof(checkmark));
    }
}