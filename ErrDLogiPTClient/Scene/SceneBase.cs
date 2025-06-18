using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Registry;
using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Service;
using ErrDLogiPTClient.Wrapper;
using GHEngine;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public IGenericServices SceneServices { get; private init; }
    public IEnumerable<ISceneComponent> Components => _components;
    public int ComponentCount => _components.Count;


    // Protected fields.
    protected IGenericServices GlobalGameServices { get; private init; }


    // Private fields.
    private readonly List<ISceneComponent> _components = new();
    private object _lockObject = new();
    private SceneLoadStatus _loadStatus = SceneLoadStatus.NotLoaded;


    // Constructors.
    public SceneBase(IGenericServices globalServices)
    {
        GlobalGameServices = globalServices;
        SceneServices = new WrapperServices();
        InitializeSceneServices();
    }


    // Protected methods.
    protected virtual void HandleLoadPreComponent() { }
    protected virtual void HandleLoadPostComponent() { }
    protected virtual void HandleUnloadPreComponent() { }
    protected virtual void HandleUnloadPostComponent() { }
    protected virtual void HandleStartPreComponent() { }
    protected virtual void HandleStartPostComponent() { }
    protected virtual void HandleEndPreComponent() { }
    protected virtual void HandleEndPostComponent() { }
    protected virtual void HandleUpdatePreComponent(IProgramTime time) { }
    protected virtual void HandleUpdatePostComponent(IProgramTime time) { }


    // Private methods.
    private void InitializeSceneServices()
    {
        SceneServices.Set<ILogger>(new WrappedServiceLogger(GlobalGameServices));
        SceneServices.Set<IGamePathStructure>(new WrappedServiceGamePathStructure(GlobalGameServices));
        SceneServices.Set<IDisplay>(new WrapperServiceDisplay(GlobalGameServices));
        SceneServices.Set<IUserInput>(new WrapperServiceUserInput(GlobalGameServices));
        SceneServices.Set<ILogiSoundEngine>(new WrappedServiceLogiSoundEngine(GlobalGameServices));
        SceneServices.Set<ISceneAssetProvider>(new DefaultSceneAssetProvider(this, GlobalGameServices));
        SceneServices.Set<IFrameExecutor>(new WrappedServiceFrameExecutor(GlobalGameServices));
        SceneServices.Set<ISceneExecutor>(new WrappedServiceSceneExecutor(GlobalGameServices));
        SceneServices.Set<IModManager>(new WrappedServiceModManager(GlobalGameServices));
        SceneServices.Set<ILogiAssetManager>(new WrappedServicLogiAssetManager(GlobalGameServices));
        SceneServices.Set<IModifiableProgramTime>(new WrappedServiceModifiableProgramTime(GlobalGameServices));
        SceneServices.Set<IUIElementFactory>(GlobalGameServices.GetRequired<ISceneFactoryProvider>().GetUIElementFactory(this));
        SceneServices.Set<IGameRegistryStorage>(new WrappedServiceGameRegistryStorage(GlobalGameServices));

        foreach (var Entry in SceneServices.Services)
        {
            if (Entry.Value is IServiceWrapperObject WrapperObject)
            {
                WrapperObject.InitializeWrapper();
            }
        }
    }

    protected void DeinitializeSceneServices()
    {
        foreach (var Entry in SceneServices.Services)
        {
            if (Entry.Value is IServiceWrapperObject WrapperObject)
            {
                WrapperObject.DeinitializeWrapper();
            }
        }
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

    public void OnEnd()
    {
        HandleEndPreComponent();
        foreach (ISceneComponent Component in _components)
        {
            Component.OnEnd();
        }
        HandleEndPostComponent();
        DeinitializeSceneServices();
    }

    public void OnStart()
    {
        HandleStartPreComponent();
        foreach (ISceneComponent Component in _components)
        {
            Component.OnStart();
        }
        HandleStartPostComponent();
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

    public void Update(IProgramTime time)
    {
        HandleUpdatePreComponent(time);
        foreach (ISceneComponent Component in _components)
        {
            Component.Update(time);
        }
        HandleUpdatePostComponent(time);
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

        if (AddArgs.IsCancelled || (AddArgs.Component == null))
        {
            AddArgs.ExecuteActions();
            return;
        }

        _components.Insert(index, AddArgs.Component);
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

        if (RemoveArgs.Component != null)
        {
            _components.Remove(RemoveArgs.Component);
        }
        
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