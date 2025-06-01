using ErrDLogiPTClient.Scene.Event;
using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface ISceneComponent : ITimeUpdatable, ISceneComponentHolder
{
    // Fields.
    IGameScene Scene { get; }
    GenericServices SceneServices { get; }
    event EventHandler<SubComponentAddEventArgs>? ComponentAdd;
    event EventHandler<SubComponentRemoveEventArgs>? ComponentRemove;


    // Methods.
    void OnStart();
    void OnEnd();
    void OnLoad();
    void OnUnload();
}