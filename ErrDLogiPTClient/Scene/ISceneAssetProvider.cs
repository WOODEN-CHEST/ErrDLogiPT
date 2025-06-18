using ErrDLogiPTClient.Wrapper;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Frame.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface ISceneAssetProvider
{
    // Methods.
    T GetAsset<T>(AssetType type, string name) where T : class;
    void ReleaseAsset(AssetType type, string name);
    void ReleaseAsset(object asset);
    void ReleaseAllAssets();


    void Initialize();
    void Deinitialize();
    void UpdateAssets();

    void RegisterRenderedItem(TextBox item);
    void RegisterRenderedItem(SpriteItem item);
    void UnregisterRenderedItem(TextBox item);
    void UnregisterRenderedItem(SpriteItem item);
}