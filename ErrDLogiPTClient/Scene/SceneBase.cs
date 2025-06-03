using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI;
using GHEngine;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using System;
using System.Collections.Generic;
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

    public GenericServices SceneServices { get; private init; }
    public IEnumerable<ISceneComponent> Components => _components;
    public int ComponentCount => _components.Count;


    // Protected fields.
    protected GenericServices GlobalServices { get; private init; }


    // Private fields.
    private readonly List<ISceneComponent> _components = new();
    private object _lockObject = new();
    private SceneLoadStatus _loadStatus = SceneLoadStatus.NotLoaded;


    // Constructors.
    public SceneBase(GenericServices globalServices)
    {
        GlobalServices = globalServices;
        SceneServices = new();
        InitializeSceneServices();
    }


    // Protected methods.
    protected virtual void HandleLoadPreComponent() { }
    protected virtual void HandleLoadPostComponent() { }
    protected virtual void HandleUnloadPreComponent() { }
    protected virtual void HandleUnloadPostComponent() { }


    // Private methods.
    private void InitializeSceneServices()
    {
        SceneServices.Set<ILogger>(GlobalServices.Get<ILogger>());
        SceneServices.Set<IGamePathStructure>(GlobalServices.Get<IGamePathStructure>());
        SceneServices.Set<IDisplay>(GlobalServices.Get<IDisplay>());
        SceneServices.Set<IUserInput>(GlobalServices.Get<IUserInput>());
        SceneServices.Set<ILogiSoundEngine>(GlobalServices.Get<ILogiSoundEngine>());
        SceneServices.Set<ISceneAssetProvider>(new DefaultSceneAssetProvider(this, GlobalServices));
        SceneServices.Set<IFrameExecutor>(GlobalServices.Get<IFrameExecutor>());
        SceneServices.Set<ISceneExecutor>(GlobalServices.Get<ISceneExecutor>());
        SceneServices.Set<IModManager>(GlobalServices.Get<IModManager>());
        SceneServices.Set<ILogiAssetLoader>(GlobalServices.Get<ILogiAssetLoader>());
        SceneServices.Set<IModifiableProgramTime>(GlobalServices.Get<IModifiableProgramTime>());
        SceneServices.Set<IUIElementFactory>(GlobalServices.GetRequired<ISceneFactoryProvider>().GetUIElementFactory(this));
        SceneServices.Set<IAppStateController>(GlobalServices.Get<IAppStateController>());
    }


    // Inherited methods.
    public void Load()
    {
        if (LoadStatus != SceneLoadStatus.NotLoaded)
        {
            return;
        }
 
        LoadStatus = SceneLoadStatus.Loading;

        ISceneAssetProvider? AssetProvider = SceneServices.Get<ISceneAssetProvider>();
        AssetProvider?.Initialize();

        HandleLoadPreComponent();
        foreach (ISceneComponent Component in _components)
        {
            Component.OnLoad();
        }
        HandleLoadPostComponent();

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

        ISceneAssetProvider? AssetProvider = SceneServices.Get<ISceneAssetProvider>();
        AssetProvider?.ReleaseAllAssets();
        AssetProvider?.Deinitialize();
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