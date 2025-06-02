using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class DropdownListElement<T>
{
    // Fields.
    public string DisplayName { get; }
    public T Value { get; }
    public Color? NormalColorOverride { get; init; } = null;
    public Color? HoverColorOverride { get; init; } = null;
    public Color? ClickColorOverride { get; init; } = null;
    public Color? SelectedColorOverride { get; init; } = null;
    public Color? UnavailableColorOverride { get; init; } = null;
    public bool IsSelectable { get; init; } = true;


    // Constructors.
    public DropdownListElement(string displayName, T value)
    {
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Value = value;
    }
}