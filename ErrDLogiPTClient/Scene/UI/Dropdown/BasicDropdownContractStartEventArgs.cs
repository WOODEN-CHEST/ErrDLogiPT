using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Dropdown;

public class BasicDropdownContractStartEventArgs<T> : BasicDropdownEventArgs<T>
{
    public BasicDropdownContractStartEventArgs(IBasicDropdownList<T> dropdown) : base(dropdown) { }
}
