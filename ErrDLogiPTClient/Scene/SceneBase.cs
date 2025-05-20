using GHEngine;
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
    protected ISceneAssetProvider AssetProvider { get; private init; }


    // Constructors.
    public SceneBase(IGameServices services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        AssetProvider = new DefaultSceneAssetProvider(this, services.AssetProvider);
    }


    // Protected methods.
    protected virtual void HandleLoad() { }

    protected virtual void HandleUnload() { }


    // Inherited methods.
    public void Load()
    {
        HandleLoad();
        IsLoaded = true;
        SceneLoadFinish?.Invoke(this, new(this));
    }

    public virtual void OnEnd() { }

    public virtual void OnStart() { }

    public void Unload()
    {
        HandleUnload();
        Services.AssetProvider.ReleaseUserAssets(this);
    }

    public void Update(IProgramTime time) { }
}