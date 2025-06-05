using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Button;

public class BasicButtonHoverStartEventArgs : BasicButtonEventArgs
{    
    // Constructors.
    public BasicButtonHoverStartEventArgs(IBasicButton button) : base(button)
    {
    }
}