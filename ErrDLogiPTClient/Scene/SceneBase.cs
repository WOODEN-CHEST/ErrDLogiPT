using ErrDLogiPTClient.Scene.Event;
using GHEngine;
using GHEngine.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public event EventHandler<SceneComponentAddEventArgs>? SceneComponentAdd;
    public event EventHandler<SceneComponentRemoveEventArgs>? SceneComponentRemove;

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
        AssetProvider = new DefaultSceneAssetProvider(this, services.AssetProvider, services.Display);
    }


    // Protected methods.
    protected virtual void HandleLoadPreComponent() { }
    protected virtual void HandleLoadPostComponent() { }
    protected virtual void HandleUnloadPreComponent() { }
    protected virtual void HandleUnloadPostComponent() { }


    // Inherited methods.
    public void Load()
    {
        if (LoadStatus != SceneLoadStatus.NotLoaded)
        {
            return;
        }
 
        LoadStatus = SceneLoadStatus.Loading;

        AssetProvider.Initialize();

        HandleLoadPreComponent();
        foreach (ISceneComponent Component in _components)
        {
            Component.OnLoad();
        }
        HandleLoadPostComponent();

        AssetProvider.UpdateAssets();

        LoadStatus = SceneLoadStatus.FinishedLoading;
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
        HandleUnloadPreComponent();
        foreach (ISceneComponent Component in _components)
        {
            Component.OnUnload();
        }
        HandleUnloadPostComponent();
        AssetProvider.ReleaseAllAssets();
        AssetProvider.Deinitialize();
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
        InsertComponent(component, ComponentCount);
    }

    public ISceneComponent GetComponent(int index)
    {
        return _components[index];
    }

    public void InsertComponent(ISceneComponent component, int index)
    {
        ArgumentNullException.ThrowIfNull(component, nameof(component));

        SceneComponentAddEventArgs AddArgs = new(this, component);
        SceneComponentAdd?.Invoke(this, AddArgs);

        if (AddArgs.IsCancelled)
        {
            AddArgs.ExecuteActions();
            return;
        }

        _components.Insert(index, component);
        AddArgs.ExecuteActions();
    }

    public void RemoveComponent(ISceneComponent component)
    {
        ArgumentNullException.ThrowIfNull(component, nameof(component));

        SceneComponentRemoveEventArgs RemoveArgs = new(this, component);
        SceneComponentRemove?.Invoke(this, RemoveArgs);

        if (RemoveArgs.IsCancelled)
        {
            RemoveArgs.ExecuteActions();
            return;
        }

        _components.Remove(component);
        RemoveArgs.ExecuteActions();
    }

    public void ClearComponents()
    {
        foreach (ISceneComponent Component in _components.ToArray())
        {
            RemoveComponent(Component);
        }
    }
}