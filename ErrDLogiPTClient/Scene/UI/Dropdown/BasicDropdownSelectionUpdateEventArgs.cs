using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI.Dropdown;

public class BasicDropdownSelectionUpdateEventArgs<T> : BasicDropdownEventArgs<T>
{
    // Fields.
    public IEnumerable<DropdownListElement<T>> SelectedElements { get; }


    // Constructors.
    public BasicDropdownSelectionUpdateEventArgs(IBasicDropdownList<T> dropdown,
        IEnumerable<DropdownListElement<T>> selectedElements) : base(dropdown)
    {
        SelectedElements = selectedElements ?? throw new ArgumentNullException(nameof(selectedElements));
    }
}