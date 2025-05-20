using GHEngine.Assets;
using GHEngine.Assets.Def;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface ISceneAssetProvider
{
    T? GetAsset<T>(AssetType type, string name) where T : class;
    void ReleaseAsset(AssetType type, string name);
    void ReleaseAsset(object asset);
    void ReleaseAllAssets();
}