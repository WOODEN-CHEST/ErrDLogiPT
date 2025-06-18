using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Service;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using Microsoft.Xna.Framework.Graphics;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

namespace ErrDLogiPTClient;

public class DefaultLogiAssetManager : ILogiAssetManager
{
    // Fields.
    public virtual IEnumerable<AssetDefinition> Definitions => DefinitionCollection;


    // Protected fields.
    protected virtual IGenericServices Services
    {
        get => _services;
        set => _services = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected virtual IAssetDefinitionCollection DefinitionCollection
    {
        get => _definitionCollection;
        set => _definitionCollection = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected virtual IAssetLoader AssetLoader
    {
        get => _assetLoader;
        set => _assetLoader = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected virtual IAssetProvider AssetProvider
    {
        get => _assetProvider;
        set => _assetProvider = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected virtual IAssetStreamOpener StreamOpener
    {
        get => _streamOpener;
        set => _streamOpener = value ?? throw new ArgumentNullException(nameof(value));
    }


    // Private fields.
    private IGenericServices _services;

    private IAssetDefinitionCollection _definitionCollection = new GHAssetDefinitionCollection();
    private IAssetLoader _assetLoader ;
    private IAssetProvider _assetProvider;
    private IAssetStreamOpener _streamOpener = new GHAssetStreamOpener();


    // Constructors.
    public DefaultLogiAssetManager(GlobalServices globalServices,
        GraphicsDevice graphicsDevice,
        WaveFormat audioFormat)
    {
        Services = globalServices;

        GHGenericAssetLoader Loader = new();
        Loader.SetTypeLoader(AssetType.Animation, new AnimationLoader(_streamOpener, graphicsDevice));
        Loader.SetTypeLoader(AssetType.Font, new FontLoader(_streamOpener, graphicsDevice));
        Loader.SetTypeLoader(AssetType.Sound, new SoundLoader(_streamOpener, audioFormat));
        _assetLoader = Loader;

        _assetProvider = new GHAssetProvider(AssetLoader, DefinitionCollection, null);
    }


    // Protected methods.
    protected virtual void OnLoadAssetDefinitions() { }

    protected virtual void OnAssetPathSet(List<string> assetPaths) { }


    // Inherited methods.
    public virtual void LoadAssetDefinitions()
    {
        IAllAssetDefinitionConverter DefinitionReader  = new JSONAllAssetDefinitionConverter();
        DefinitionReader.Read(DefinitionCollection, Services.GetRequired<IGamePathStructure>().AssetDefRoot);

        foreach (ModPackage Package in Services.GetRequired<IModManager>().Mods)
        {
            foreach (AssetDefinition Definition in Package.AssetDefinitions)
            {
                DefinitionCollection.Add(Definition);
            }
        }

        OnLoadAssetDefinitions();
    }

    public virtual void AddAssetDefinition(AssetDefinition definition)
    {
        DefinitionCollection.Add(definition);
    }

    public virtual void ClearAssetDefinitions()
    {
        DefinitionCollection.Clear();
    }

    public virtual void RemoveAssetDefinition(AssetDefinition definition)
    {
        DefinitionCollection.Remove(definition);
    }

    public virtual T? GetAsset<T>(object user, AssetType type, string name) where T : class
    {
        return _assetProvider.GetAsset<T>(user, type, name);
    }

    public virtual void ReleaseAllAssets()
    {
        AssetProvider.ReleaseAllAssets();
    }

    public virtual void ReleaseAsset(object user, AssetType type, string name)
    {
        AssetProvider.ReleaseAsset(user, type, name);
    }

    public virtual void ReleaseAsset(object user, object asset)
    {
        AssetProvider.ReleaseAsset(user, asset);
    }

    public virtual void ReleaseUserAssets(object user)
    {
        AssetProvider.ReleaseUserAssets(user);
    }

    public virtual void RemoveAssetMemoryStream(string path)
    {
        StreamOpener.RemoveMemoryStream(path, true);
    }

    public virtual void SetAssetMemoryStream(string path, Stream stream)
    {
        StreamOpener?.SetMemoryStream(path, stream);
    }

    public virtual void SetAssetRootPaths(params string[] resourcePackDirNames)
    {
        ArgumentNullException.ThrowIfNull(resourcePackDirNames, nameof(resourcePackDirNames));
        List<string> AssetPaths = new();

        AssetPaths.AddRange(resourcePackDirNames);

        foreach (ModPackage Package in Services.GetRequired<IModManager>().Mods)
        {
            if (Package.Structure.AssetRoot != null)
            {
                AssetPaths.Add(Package.Structure.AssetRoot);
            }
        }

        OnAssetPathSet(AssetPaths);
        AssetPaths.Add(Services.GetRequired<IGamePathStructure>().AssetValueRoot);

        StreamOpener.SetAssetPaths(AssetPaths.ToArray());
    }
}