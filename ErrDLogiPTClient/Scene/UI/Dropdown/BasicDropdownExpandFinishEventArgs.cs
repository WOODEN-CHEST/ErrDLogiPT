using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Dropdown;

public class BasicDropdownExpandFinishEventArgs<T> : BasicDropdownEventArgs<T>
{
    public BasicDropdownExpandFinishEventArgs(IBasicDropdownList<T> dropdown) : base(dropdown) { }
}