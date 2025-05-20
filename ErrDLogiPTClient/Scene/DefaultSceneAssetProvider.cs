using GHEngine.Assets;
using GHEngine.Assets.Def;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public class DefaultSceneAssetProvider : ISceneAssetProvider
{
    // Private fields.
    private readonly IGameScene _scene;
    private readonly IAssetProvider _assetProvider;

    // Constructors.
    public DefaultSceneAssetProvider(IGameScene scene, IAssetProvider assetProvider)
    {
        _scene = scene ?? throw new ArgumentNullException(nameof(scene));
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
    }

    // Inherited methods.
    public T? GetAsset<T>(AssetType type, string name) where T : class
    {
        T? Asset = _assetProvider.GetAsset<T>(_scene, type, name);

        if (Asset == null)
        {
            throw new SceneAssetMissingException($"Missing scene asset of type \"{type}\", " +
                $"name \"{name}\" for scene {_scene.GetType().FullName}");
        }

        return Asset;
    }

    public void ReleaseAllAssets()
    {
        _assetProvider.ReleaseUserAssets(_scene);
    }

    public void ReleaseAsset(AssetType type, string name)
    {
        _assetProvider.ReleaseAsset(_scene, type, name);
    }

    public void ReleaseAsset(object asset)
    {
        _assetProvider.ReleaseAsset(_scene, asset);
    }
}