using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Button;

public class BasicButtonScrollEventArgs : BasicButtonEventArgs
{
    // Fields.
    public int ScrollAmount { get; set; }


    // Constructors.
    public BasicButtonScrollEventArgs(IBasicButton button, int scrollAmount) : base(button)
    {
        ScrollAmount = scrollAmount;
    }
}