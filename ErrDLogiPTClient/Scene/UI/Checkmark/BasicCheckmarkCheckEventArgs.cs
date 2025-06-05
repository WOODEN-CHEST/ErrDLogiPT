using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Checkmark;

public class BasicCheckmarkCheckEventArgs : BasicCheckmarkEventArgs
{
    // Fields.
    public bool IsChecked { get; set; }


    // Constructors.
    public BasicCheckmarkCheckEventArgs(IBasicCheckmark checkmark, bool isChecked) : base(checkmark)
    {
        IsChecked = isChecked;
    }
}