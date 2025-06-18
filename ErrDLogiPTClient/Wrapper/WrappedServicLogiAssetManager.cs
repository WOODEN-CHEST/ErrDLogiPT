using ErrDLogiPTClient.Service;
using GHEngine.Assets.Def;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrappedServicLogiAssetManager : ServiceWrapper<ILogiAssetManager>, ILogiAssetManager
{
    // Fields.
    public IEnumerable<AssetDefinition> Definitions => ServiceObject.Definitions;


    // Constructors.
    public WrappedServicLogiAssetManager(IGenericServices services) : base(services) { }


    // Inherited methods.
    public void AddAssetDefinition(AssetDefinition definition)
    {
        ServiceObject.AddAssetDefinition(definition);
    }

    public void ClearAssetDefinitions()
    {
        ServiceObject.ClearAssetDefinitions();
    }

    public T? GetAsset<T>(object user, AssetType type, string name) where T : class
    {
        return ServiceObject.GetAsset<T>(user, type, name);
    }

    public void LoadAssetDefinitions()
    {
        ServiceObject.LoadAssetDefinitions();
    }

    public void ReleaseAllAssets()
    {
        ServiceObject.ReleaseAllAssets();
    }

    public void ReleaseAsset(object user, AssetType type, string name)
    {
        ServiceObject.ReleaseAsset(user, type, name);
    }

    public void ReleaseAsset(object user, object asset)
    {
        ServiceObject.ReleaseAsset(user, asset);
    }

    public void ReleaseUserAssets(object user)
    {
        ServiceObject.ReleaseUserAssets(user);
    }

    public void RemoveAssetDefinition(AssetDefinition definition)
    {
        ServiceObject.RemoveAssetDefinition(definition);
    }

    public void RemoveAssetMemoryStream(string path)
    {
        ServiceObject.RemoveAssetMemoryStream(path);
    }

    public void SetAssetMemoryStream(string path, Stream stream)
    {
        ServiceObject.SetAssetMemoryStream(path, stream);
    }

    public void SetAssetRootPaths(string[] resourcePackDirNames)
    {
        ServiceObject.SetAssetRootPaths(resourcePackDirNames);
    }
}