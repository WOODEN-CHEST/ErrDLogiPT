using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Dropdown;

public class BasicDropdownContractFinishEventArgs<T> : BasicDropdownEventArgs<T>
{
    public BasicDropdownContractFinishEventArgs(IBasicDropdownList<T> dropdown) : base(dropdown) { }
}
