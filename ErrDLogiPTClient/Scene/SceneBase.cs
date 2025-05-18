using GHEngine.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public abstract class SceneBase : IGameScene
{
    // Fields.
    public bool IsLoaded { get; private set; } = false;

    public event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;


    // Protected fields.
    protected IGameServices Services { get; private init; }


    // Constructors.
    public SceneBase(IGameServices services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }


    // Protected methods.
    protected virtual void HandleLoad(IAssetProvider assetProvider) { }

    protected void HandleUnload(IAssetProvider assetProvider) { }


    // Inherited methods.
    public void Load()
    {
        HandleLoad(Services.AssetProvider);
        IsLoaded = true;
    }

    public virtual void OnEnd() { }

    public virtual void OnStart() { }

    public void Unload()
    {
        HandleUnload(Services.AssetProvider);
        Services.AssetProvider.ReleaseUserAssets(this);
        IsLoaded = false;
    }
}