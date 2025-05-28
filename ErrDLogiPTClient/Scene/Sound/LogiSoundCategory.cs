using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

public class LogiSoundCategory
{
    // Static fields.
    public static LogiSoundCategory UI { get; }
        = new("User Interface", "UI elements like buttons, sliders, drop-downs, etc.");

    public static LogiSoundCategory Music { get; }
        = new("Music", "Background music");

    public static LogiSoundCategory OS { get; }
        = new("Operating System", "Sounds coming from a virtual Logi operating system.");



    // Fields.
    public string Name { get; private init; }
    public string Description { get; private init; }


    // Constructors.
    public LogiSoundCategory(string name, string description)
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