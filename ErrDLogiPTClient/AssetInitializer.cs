using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public class AssetInitializer
{
    // Private fields.
    private readonly ILogger? _logger;


    // Constructors.
    public AssetInitializer(ILogger? logger)
    {
        _logger = logger;
    }


    // Methods.
    public void InitializeAssets(IGamePathStructure structure, 
        IAssetDefinitionCollection assets,
        IAllAssetDefinitionConverter definitionConverter)
    {

    }
}