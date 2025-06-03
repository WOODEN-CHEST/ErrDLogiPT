using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Button;

public class BasicButtonHoverEndEventArgs : BasicButtonEventArgs
{
    // Fields.
    public IPreSampledSound? Sound { get; set; }


    // Constructors.
    public BasicButtonHoverEndEventArgs(IBasicButton button) : base(button)
    {
    }
}