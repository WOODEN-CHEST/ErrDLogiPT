using GHEngine.Assets.Def;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public interface ILogiAssetManager
{
    // Fields.
    IEnumerable<AssetDefinition> Definitions { get; }


    // Methods.
    void LoadAssetDefinitions();
    void AddAssetDefinition(AssetDefinition definition);
    void RemoveAssetDefinition(AssetDefinition definition);
    void ClearAssetDefinitions();

    void SetAssetMemoryStream(string path, Stream stream);
    void RemoveAssetMemoryStream(string path);
    void SetAssetRootPaths(string[] resourcePackDirNames);

    T? GetAsset<T>(object user, AssetType type, string name) where T : class;
    void ReleaseAsset(object user, AssetType type, string name);
    void ReleaseAsset(object user, object asset);
    void ReleaseUserAssets(object user);
    void ReleaseAllAssets();
}