using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Sound;

/// <summary>
/// As can be guessed by the name, this represent a sound category in the game.
/// <para>It is used by the sound engine <see cref="ILogiSoundEngine"/> to allow changing the volume of
/// each sound category.</para>
/// <para>This class is not an enum so that it is possible for modders to add more sound categories if they wish to, and
/// so that each sound category could have properties like name and description.</para>
/// </summary>
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

    /// <summary>
    /// The human-friendly name of the category.
    /// </summary>
    public string Name { get; private init; }

    /// <summary>
    /// The human-friendly description of what this category affects.
    /// </summary>
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