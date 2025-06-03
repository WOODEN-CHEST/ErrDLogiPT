using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Dropdown;

public class BasicDropdownEventArgs<T> : CancellableEventBase
{
    // Fields.
    public IBasicDropdownList<T> Dropdown { get; }


    // Constructors.
    public BasicDropdownEventArgs(IBasicDropdownList<T> dropdown)
    {
        Dropdown = dropdown ?? throw new ArgumentNullException(nameof(dropdown));
    }
}