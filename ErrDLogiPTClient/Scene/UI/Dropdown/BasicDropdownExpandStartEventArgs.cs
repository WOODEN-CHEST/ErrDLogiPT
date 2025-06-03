using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Dropdown;

public class BasicDropdownExpandStartEventArgs<T> : BasicDropdownEventArgs<T>
{
    public BasicDropdownExpandStartEventArgs(IBasicDropdownList<T> dropdown) : base(dropdown) { }
}