using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public abstract class SceneComponentBase<T> : ISceneComponent where T : IGameScene
{
    // Protected fields.
    protected T Scene { get; private init; }
    protected List<ISceneComponent> SubComponents { get; } = new();


    // Constructors.
    public SceneComponentBase(T scene)
    {
        Scene = scene ?? throw new ArgumentNullException(nameof(scene));
    }


    // Inherited methods.
    public virtual void OnEnd()
    {
        foreach (ISceneComponent Component in SubComponents)
        {
            Component.OnEnd();
        }
    }

    public virtual void OnLoad()
    {
        foreach (ISceneComponent Component in SubComponents)
        {
            Component.OnLoad();
        }
    }

    public virtual void OnStart()
    {
        foreach (ISceneComponent Component in SubComponents)
        {
            Component.OnStart();
        }
    }

    public virtual void OnUnload()
    {
        foreach (ISceneComponent Component in SubComponents)
        {
            Component.OnUnload();
        }
    }

    public virtual void Update(IProgramTime time)
    {
        foreach (ISceneComponent Component in SubComponents)
        {
            Component.Update(time);
        }
    }
}