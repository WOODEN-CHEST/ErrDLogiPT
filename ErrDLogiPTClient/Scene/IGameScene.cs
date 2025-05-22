using ErrDLogiPTClient.Scene.Event;
using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface IGameScene : ITimeUpdatable
{
    // Fields.
    event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;
    event EventHandler<SceneComponentPreAddEventArgs>? SceneComponentPreAdd;
    event EventHandler<SceneComponentPreAddEventArgs>? SceneComponentPostAdd;
    event EventHandler<SceneComponentPostRemoveEventArgs>? SceneComponentPreRemove;
    event EventHandler<SceneComponentPostRemoveEventArgs>? SceneComponentPostRemove;
    SceneLoadStatus LoadStatus { get; }
    IEnumerable<ISceneComponent> Components { get; }
    int ComponentCount { get; }
    GameServices Services { get; }
    ISceneAssetProvider AssetProvider { get; }


    // Methods.
    void Load();
    void OnStart();
    void OnEnd();
    void Unload();
    void AddComponent(ISceneComponent component);
    void GetComponent(int index);
    void InsertComponent(ISceneComponent component, int index);
    void RemoveComponent(ISceneComponent component);
    void ClearComponents();
}