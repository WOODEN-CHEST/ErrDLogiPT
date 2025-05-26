using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public class SceneSoundCategory
{
    // Static fields.
    public static SceneSoundCategory UI { get; }
        = new("User Interface", "UI elements like buttons, sliders, drop-downs, etc.");

    public static SceneSoundCategory Music { get; }
        = new("Music", "Background music");

    public static SceneSoundCategory OS { get; }
        = new("Operating System", "Sounds coming from a virtual Logi operating system.");



    // Fields.
    public string Name { get; private init; }
    public string Description { get; private init; }


    // Constructors.
    public SceneSoundCategory(string name, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }


    // Inherited methods.
    public override string ToString()
    {
        return $"Sound Category \"{Name}\": \"{Description}\"";
    }
}