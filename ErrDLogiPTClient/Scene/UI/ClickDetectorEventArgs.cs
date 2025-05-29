using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public abstract class ClickDetectorEventArgs : EventArgs
{
    // Fields.
    public ClickDetector Detector { get; private init; }


    // Constructors.
    public ClickDetectorEventArgs(ClickDetector detector)
    {
        Detector = detector ?? throw new ArgumentNullException(nameof(detector));
    }
}