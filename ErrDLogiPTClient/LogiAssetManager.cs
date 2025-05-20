using ErrDLogiPTClient.Mod;
using GHEngine.Assets.Def;
using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public class LogiAssetManager : ILogiAssetManager
{
    // Private fields.
    private readonly IGameServices _services;
    private readonly IModManager _modManager;


    // Constructors.
    public LogiAssetManager(IGameServices services, IModManager modManager)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _modManager = modManager ?? throw new ArgumentNullException(nameof(modManager));
    }


    // Inherited methods.
    public void LoadAssetDefinitions()
    {
        IAssetDefinitionCollection AssetDefinitions = _services.AssetDefinitions;
        AssetDefinitions.Clear();

        IAllAssetDefinitionConverter DefinitionReader = new JSONAllAssetDefinitionConverter(_services.Logger);
        DefinitionReader.Read(AssetDefinitions, _services.Structure.AssetDefRoot);

        foreach (ModPackage Package in _modManager.Mods)
        {

        }
    }

    public void SetUsedResourcePacks(params string[] resourcePackDirNames)
    {
        throw new NotImplementedException();
    }
}