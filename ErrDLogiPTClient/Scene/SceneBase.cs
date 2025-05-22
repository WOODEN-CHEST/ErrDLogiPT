using ErrDLogiPTClient.Scene.Event;
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
    public SceneLoadStatus LoadStatus
    {
        get
        {
            lock (_lockObject)
            {
                return _loadStatus;
            }
        }
        private set
        {
            lock (_lockObject)
            {
                _loadStatus = value;
            }
        }
    }

    public event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;
    public event EventHandler<SceneComponentPreAddEventArgs>? SceneComponentPreAdd;
    public event EventHandler<SceneComponentPreRemoveEventArgs>? SceneComponentPreRemove;
    public event EventHandler<SceneComponentPreAddEventArgs>? SceneComponentPostAdd;
    public event EventHandler<SceneComponentPostRemoveEventArgs>? SceneComponentPostRemove;

    public GameServices Services { get; private init; }
    public ISceneAssetProvider AssetProvider { get; private init; }
    public IEnumerable<ISceneComponent> Components => _components;
    public int ComponentCount => _components.Count;


    // Private fields.
    private readonly List<ISceneComponent> _components = new();
    private object _lockObject = new();
    private SceneLoadStatus _loadStatus = SceneLoadStatus.NotLoaded;


    // Constructors.
    public SceneBase(GameServices services)
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
        if (LoadStatus != SceneLoadStatus.NotLoaded)
        {
            return;
        }

        LoadStatus = SceneLoadStatus.Loading;
        HandleLoad();
        foreach (ISceneComponent Component in _components)
        {
            Component.OnLoad();
        }
        LoadStatus = SceneLoadStatus.Loaded;
        SceneLoadFinish?.Invoke(this, new(this));
    }

    public virtual void OnEnd()
    {
        foreach (ISceneComponent Component in _components)
        {
            Component.OnEnd();
        }
    }

    public virtual void OnStart()
    {
        foreach (ISceneComponent Component in _components)
        {
            Component.OnStart();
        }
    }

    public void Unload()
    {
        HandleUnload();
        foreach (ISceneComponent Component in _components)
        {
            Component.OnUnload();
        }
        Services.AssetProvider.ReleaseUserAssets(this);
    }

    public virtual void Update(IProgramTime time)
    {
        foreach (ISceneComponent Component in _components)
        {
            Component.Update(time);
        }
    }

    public void AddComponent(ISceneComponent component)
    {
        ArgumentNullException.ThrowIfNull(component, nameof(component));

        SceneComponentPreAddEventArgs AddArgs = new(this, component);
        SceneComponentPreAdd?.Invoke(this, AddArgs);

        if (!AddArgs.IsCancelled)
        {
            _components.Add(component);
            SceneComponentPostAdd?.Invoke(this, new(this, component));
        }
    }

    public void GetComponent(int index)
    {
        throw new NotImplementedException();
    }

    public void InsertComponent(ISceneComponent component, int index)
    {
        throw new NotImplementedException();
    }

    public void RemoveComponent(ISceneComponent component)
    {
        throw new NotImplementedException();
    }

    public void ClearComponents()
    {
        throw new NotImplementedException();
    }
}