using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Button;

public class BasicButtonHoverEndEventArgs : BasicButtonEventArgs
{
    // Constructors.
    public BasicButtonHoverEndEventArgs(IBasicButton button) : base(button)
    {
    }
}