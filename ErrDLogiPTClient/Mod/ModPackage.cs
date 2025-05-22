using GHEngine.Assets.Def;
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
    public IModPathStructure Structure { get; private init; }
    public IAssetDefinitionCollection AssetDefinitions { get; }


    // Constructors.
    public ModPackage(IModPathStructure structure,
        string name, 
        string description,
        IGameMod entryPoint,
        IAssetDefinitionCollection assetDefinitions)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Structure = structure ?? throw new ArgumentNullException(nameof(structure));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        EntryPointObject = entryPoint ?? throw new ArgumentNullException(nameof(entryPoint));
        AssetDefinitions = assetDefinitions ?? throw new ArgumentNullException(nameof(assetDefinitions));
    }
}