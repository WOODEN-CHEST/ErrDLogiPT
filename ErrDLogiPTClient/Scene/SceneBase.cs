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
    public bool IsLoaded
    {
        get
        {
            lock (_lockObject)
            {
                return _isLoaded;
            }
        }
        private set
        {
            lock (_lockObject)
            {
                _isLoaded = value;
            }
        }
    }

    public event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;


    // Protected fields.
    protected IGameServices Services { get; private init; }
    protected ISceneAssetProvider AssetProvider { get; private init; }
    protected List<ISceneComponent> Components { get; } = new();


    // Private fields.
    private object _lockObject = new();
    private bool _isLoaded = false;


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
        foreach (ISceneComponent Component in Components)
        {
            Component.OnLoad();
        }
        IsLoaded = true;
        SceneLoadFinish?.Invoke(this, new(this));
    }

    public virtual void OnEnd()
    {
        foreach (ISceneComponent Component in Components)
        {
            Component.OnEnd();
        }
    }

    public virtual void OnStart()
    {
        foreach (ISceneComponent Component in Components)
        {
            Component.OnStart();
        }
    }

    public void Unload()
    {
        HandleUnload();
        foreach (ISceneComponent Component in Components)
        {
            Component.OnUnload();
        }
        Services.AssetProvider.ReleaseUserAssets(this);
    }

    public virtual void Update(IProgramTime time)
    {
        foreach (ISceneComponent Component in Components)
        {
            Component.Update(time);
        }
    }
}