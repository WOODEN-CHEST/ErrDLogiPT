using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Service;
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
    IGenericServices SceneServices { get; }
    event EventHandler<SubComponentAddEventArgs>? ComponentAdd;
    event EventHandler<SubComponentRemoveEventArgs>? ComponentRemove;


    // Methods.
    void OnStart();
    void OnEnd();
    void OnLoad();
    void OnUnload();
}