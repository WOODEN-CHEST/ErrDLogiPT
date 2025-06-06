using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Scene.Sound;
using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

/// <summary>
/// A game scene represents a separate, whole section of the game, for example the Main Menu, Level editor or in-game section.
/// <para>Each scene comes with its own services property which has scene-specific services. These services are very
/// similar to the global services, but with some services which work only for scenes
/// (for instance <see cref="ISceneAssetProvider"/>)</para>
/// <para>To not make the scene cluttered with tens of thousands of lines of code (like the UI elements, for example),
/// scenes can have "components" which are basically just smaller objects which focus on delivering
/// specific functionality to a scene, like a UI executor, audio manager, etc.</para>
/// <para>A scene should not be reused after its lifetime ends. A lifetime consists of the following calls, in order:
/// <see cref="Load"/>, <see cref="OnStart"/>, <see cref="OnEnd"/>, <see cref="Unload"/>. Once the scene is unloaded,
/// it should be discarded and not used again.</para>
/// </summary>
public interface IGameScene : ITimeUpdatable, ISceneComponentHolder
{
    // Fields.

    /// <summary>
    /// The event which is raised when a scene finishes loading. This event may or may not be raised from the main thread.
    /// </summary>
    event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;

    /// <summary>
    /// The event raised whenever a scene component is added.
    /// This event is typically raised from the thread which called the
    /// <see cref="ISceneComponentHolder.AddComponent(ISceneComponent)"/> or
    /// <see cref="ISceneComponentHolder.InsertComponent(ISceneComponent, int)"/> method.
    /// </summary>
    event EventHandler<SceneComponentAddEventArgs>? SceneComponentAdd;

    /// <summary>
    /// The event raised whenever a scene component is removed.
    /// This event is typically raised from the thread which called the
    /// <see cref="ISceneComponentHolder.RemoveComponent(ISceneComponent)"/> method or
    /// for each component the scene during a <see cref="ISceneComponentHolder.ClearComponents()"/> call.
    /// </summary>
    event EventHandler<SceneComponentRemoveEventArgs>? SceneComponentRemove;

    /// <summary>
    /// The current loading status of a scene.
    /// Implementations should make this property thread-safe.
    /// The default implementation already does this. (<see cref="ErrDLogiPTClient.Scene.SceneBase"/>)
    /// </summary>
    SceneLoadStatus LoadStatus { get; }

    /// <summary>
    /// The services used by this scene.
    /// </summary>
    GenericServices SceneServices { get; }


    // Methods.

    /// <summary>
    /// Loads the scene if it hasn't been loaded already.
    /// <para>Calling this method while the scene is loading or already has loaded should do nothing.</para>
    /// </summary>
    void Load();

    /// <summary>
    /// Method called before the first <see cref="ITimeUpdatable.Update(IProgramTime)"/> call when the scene
    /// has already loaded and is being used.
    /// </summary>
    void OnStart();

    /// <summary>
    /// Method called before the last <see cref="ITimeUpdatable.Update(IProgramTime)"/>. The scene is still
    /// loaded during the call to this method.
    /// </summary>
    void OnEnd();

    /// <summary>
    /// Unloads the scene.
    /// </summary>
    void Unload();
}