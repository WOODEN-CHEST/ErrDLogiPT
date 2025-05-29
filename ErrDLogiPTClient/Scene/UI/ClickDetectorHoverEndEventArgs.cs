using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class ClickDetectorHoverEndEventArgs : ClickDetectorEventArgs
{
    public ClickDetectorHoverEndEventArgs(ClickDetector detector) : base(detector)
    {
    }
}