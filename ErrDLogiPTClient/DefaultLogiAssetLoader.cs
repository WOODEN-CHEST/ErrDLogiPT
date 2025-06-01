using ErrDLogiPTClient.Mod;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public class DefaultLogiAssetLoader : ILogiAssetLoader
{
    // Private fields.
    private readonly GenericServices _globalServices;


    // Constructors.
    public DefaultLogiAssetLoader(GenericServices globalServices)
    {
        _globalServices = globalServices ?? throw new ArgumentNullException(nameof(globalServices));
    }


    // Inherited methods.
    public void LoadAssetDefinitions()
    {
        IAssetDefinitionCollection AssetDefinitions = _globalServices.GetRequired<IAssetDefinitionCollection>();
        AssetDefinitions.Clear();

        IAllAssetDefinitionConverter DefinitionReader  = new JSONAllAssetDefinitionConverter(_globalServices.Get<ILogger>());
        DefinitionReader.Read(AssetDefinitions, _globalServices.GetRequired<IGamePathStructure>().AssetDefRoot);

        foreach (ModPackage Package in _globalServices.GetRequired<IModManager>().Mods)
        {
            foreach (AssetDefinition Definition in Package.AssetDefinitions)
            {
                AssetDefinitions.Add(Definition);
            }
        }
    }

    public void SetAssetRootPaths(params string[] resourcePackDirNames)
    {
        ArgumentNullException.ThrowIfNull(resourcePackDirNames, nameof(resourcePackDirNames));
        List<string> AssetPaths = new();

        AssetPaths.AddRange(resourcePackDirNames);

        foreach (ModPackage Package in _globalServices.GetRequired<IModManager>().Mods)
        {
            if (Package.Structure.AssetRoot != null)
            {
                AssetPaths.Add(Package.Structure.AssetRoot);
            }
        }

        AssetPaths.Add(_globalServices.GetRequired<IGamePathStructure>().AssetValueRoot);

        IAssetStreamOpener StreamOpener = _globalServices.GetRequired<IAssetStreamOpener>();
        if (StreamOpener is GHAssetStreamOpener AdvancedStreamOpener)
        {
            AdvancedStreamOpener.SetAssetPaths(AssetPaths.ToArray());
        }
        
    }
}