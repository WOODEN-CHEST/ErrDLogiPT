using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public class ModPackage
{
    // Fields.
    public string Name { get; private init; }
    public string Description { get; private init; }
    public IGameMod EntryPointObject { get; private init; }


    // Constructors.
    public ModPackage(string name, string description, IGameMod entryPoint)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        EntryPointObject = entryPoint ?? throw new ArgumentNullException(nameof(entryPoint));
    }
}