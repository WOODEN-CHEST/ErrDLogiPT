using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Service;
using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public abstract class SceneComponentBase : ISceneComponent
{
    // Fields.
    public IGameScene Scene { get; private init; }
    public IEnumerable<ISceneComponent> Components => _subComponents;
    public int ComponentCount => _subComponents.Count;
    public IGenericServices SceneServices { get; private init; }


    // Private fields.
    private readonly List<ISceneComponent> _subComponents = new();


    // Constructors.
    public SceneComponentBase(IGameScene scene, IGenericServices services)
    {
        Scene = scene ?? throw new ArgumentNullException(nameof(scene));
        SceneServices = services ?? throw new ArgumentNullException(nameof(services));
    }

    public event EventHandler<SubComponentAddEventArgs>? ComponentAdd;
    public event EventHandler<SubComponentRemoveEventArgs>? ComponentRemove;


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


    // Inherited methods.
    public void OnEnd()
    {
        HandleEndPreComponent();
        foreach (ISceneComponent Component in _subComponents)
        {
            Component.OnEnd();
        }
        HandleEndPostComponent();
    }

    public void OnLoad()
    {
        HandleLoadPreComponent();
        foreach (ISceneComponent Component in _subComponents)
        {
            Component.OnLoad();
        }
        HandleLoadPostComponent();
    }

    public void OnStart()
    {
        HandleStartPreComponent();
        foreach (ISceneComponent Component in _subComponents)
        {
            Component.OnStart();
        }
        HandleStartPostComponent();
    }

    public void OnUnload()
    {
        HandleUnloadPreComponent();
        foreach (ISceneComponent Component in _subComponents)
        {
            Component.OnUnload();
        }
        HandleUnloadPostComponent();
    }

    public void Update(IProgramTime time)
    {
        HandleUpdatePreComponent(time);
        foreach (ISceneComponent Component in _subComponents)
        {
            Component.Update(time);
        }
        HandleUpdatePostComponent(time);
    }

    public virtual void AddComponent(ISceneComponent component)
    {
        InsertComponent(component, ComponentCount);
    }

    public virtual ISceneComponent GetComponent(int index)
    {
        return _subComponents[index];
    }

    public virtual void InsertComponent(ISceneComponent component, int index)
    {
        ArgumentNullException.ThrowIfNull(component, nameof(component));

        SubComponentAddEventArgs AddArgs = new(this, component);
        ComponentAdd?.Invoke(this, AddArgs);

        if (AddArgs.IsCancelled)
        {
            AddArgs.ExecuteActions();
            return;
        }

        _subComponents.Insert(index, component);
        AddArgs.ExecuteActions();
    }

    public virtual void RemoveComponent(ISceneComponent component)
    {
        ArgumentNullException.ThrowIfNull(component, nameof(component));

        SubComponentRemoveEventArgs RemoveArgs = new(this, component);
        ComponentRemove?.Invoke(this, RemoveArgs);

        if (RemoveArgs.IsCancelled)
        {
            RemoveArgs.ExecuteActions();
            return;
        }

        _subComponents.Remove(component);
        RemoveArgs.ExecuteActions();
    }

    public virtual void ClearComponents()
    {
        foreach (ISceneComponent Component in _subComponents.ToArray())
        {
            RemoveComponent(Component);
        }
    }
}