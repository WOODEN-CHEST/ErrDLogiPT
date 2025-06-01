using ErrDLogiPTClient.Scene.Event;
using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public abstract class SceneComponentBase<T> : ISceneComponent where T : IGameScene
{
    // Fields.
    public IGameScene Scene => TypedScene;
    public IEnumerable<ISceneComponent> Components => _subComponents;
    public int ComponentCount => _subComponents.Count;
    public GenericServices SceneServices { get; private init; }


    // Protected fields.
    protected T TypedScene { get; private init; }


    // Private fields.
    private readonly List<ISceneComponent> _subComponents = new();


    // Constructors.
    public SceneComponentBase(T scene, GenericServices services)
    {
        TypedScene = scene ?? throw new ArgumentNullException(nameof(scene));
        SceneServices = services ?? throw new ArgumentNullException(nameof(services));
    }

    public event EventHandler<SubComponentAddEventArgs>? ComponentAdd;
    public event EventHandler<SubComponentRemoveEventArgs>? ComponentRemove;


    // Inherited methods.
    public virtual void OnEnd()
    {
        foreach (ISceneComponent Component in _subComponents)
        {
            Component.OnEnd();
        }
    }

    public virtual void OnLoad()
    {
        foreach (ISceneComponent Component in _subComponents)
        {
            Component.OnLoad();
        }
    }

    public virtual void OnStart()
    {
        foreach (ISceneComponent Component in _subComponents)
        {
            Component.OnStart();
        }
    }

    public virtual void OnUnload()
    {
        foreach (ISceneComponent Component in _subComponents)
        {
            Component.OnUnload();
        }
    }

    public virtual void Update(IProgramTime time)
    {
        foreach (ISceneComponent Component in _subComponents)
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
        return _subComponents[index];
    }

    public void InsertComponent(ISceneComponent component, int index)
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

    public void RemoveComponent(ISceneComponent component)
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

    public void ClearComponents()
    {
        foreach (ISceneComponent Component in _subComponents.ToArray())
        {
            RemoveComponent(Component);
        }
    }
}