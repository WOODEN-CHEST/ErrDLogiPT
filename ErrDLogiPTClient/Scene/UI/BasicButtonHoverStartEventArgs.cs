using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class BasicButtonHoverStartEventArgs : BasicButtonEventArgs
{    
    // Fields.
    public IPreSampledSound? Sound { get; set; }


    // Constructors.
    public BasicButtonHoverStartEventArgs(UIBasicButton button) : base(button)
    {
    }
}