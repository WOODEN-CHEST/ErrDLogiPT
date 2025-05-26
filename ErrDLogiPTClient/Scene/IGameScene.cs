using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Scene.Sound;
using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface IGameScene : ITimeUpdatable, ISceneComponentHolder
{
    // Fields.
    event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;
    event EventHandler<SceneComponentAddEventArgs>? SceneComponentAdd;
    event EventHandler<SceneComponentRemoveEventArgs>? SceneComponentRemove;
    SceneLoadStatus LoadStatus { get; }
    IEnumerable<ISceneComponent> Components { get; }
    int ComponentCount { get; }
    GameServices Services { get; }
    ISceneAssetProvider AssetProvider { get; }
    ISceneSoundEngine SoundEngine { get; }


    // Methods.
    void Load();
    void OnStart();
    void OnEnd();
    void Unload();
}