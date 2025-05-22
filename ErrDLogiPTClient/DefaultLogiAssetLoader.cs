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
    private readonly IModManager _modManager;
    private readonly IGamePathStructure _structure;
    private readonly ILogger? _logger;
    private readonly IAssetDefinitionCollection _assets;
    private readonly GHAssetStreamOpener _streamOpener;


    // Constructors.
    public DefaultLogiAssetLoader(IModManager modManager,
        IGamePathStructure structure,
        ILogger? logger,
        IAssetDefinitionCollection assets,
        GHAssetStreamOpener streamOpener)
    {
        _modManager = modManager ?? throw new ArgumentNullException(nameof(modManager));
        _structure = structure ?? throw new ArgumentNullException(nameof(structure));
        _assets = assets ?? throw new ArgumentNullException(nameof(assets));
        _streamOpener = streamOpener ?? throw new ArgumentNullException(nameof(streamOpener));
        _logger = logger;
    }


    // Inherited methods.
    public void LoadAssetDefinitions()
    {
        IAssetDefinitionCollection AssetDefinitions = _assets;
        AssetDefinitions.Clear();

        IAllAssetDefinitionConverter DefinitionReader = new JSONAllAssetDefinitionConverter(_logger);
        DefinitionReader.Read(AssetDefinitions, _structure.AssetDefRoot);

        foreach (ModPackage Package in _modManager.Mods)
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

        foreach (ModPackage Package in _modManager.Mods)
        {
            if (Package.Structure.AssetRoot != null)
            {
                AssetPaths.Add(Package.Structure.AssetRoot);
            }
        }

        AssetPaths.Add(_structure.AssetValueRoot);

        _streamOpener.SetAssetPaths(AssetPaths.ToArray());
    }
}