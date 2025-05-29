using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class ClickDetectorScrollEventArgs : ClickDetectorEventArgs
{
    // Fields.
    public int ScrollAmount { get; private init; }


    // Constructors.
    public ClickDetectorScrollEventArgs(ClickDetector detector, int scrollAmount) : base(detector)
    {
        ScrollAmount = scrollAmount;
    }
}