using ErrDLogiPTClient.Scene.Event;
using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

/// <summary>
/// A scene executor updates the current scene and allows to switch which scene is currently active.
/// <para>Any implementation of a scene executor must be thread-safe.</para>
/// <para>All events are called only from the main thread.</para>
/// </summary>
public interface ISceneExecutor : ITimeUpdatable
{
    // Fields.
    /// <summary>
    /// The scene which is currently active, or <c>null</c> if no scene is active.
    /// </summary>
    IGameScene? CurrentScene { get; }

    /// <summary>
    /// Whether the next scene is loaded. A scheduled jump to the next scene executes only when it is loaded.
    /// <para>Returns <c>false</c> if there is no next scene.</para>
    /// </summary>
    bool IsNextSceneLoaded { get; }

    /// <summary>
    /// Gets whether a next scene exists, regardless whether is it loaded or not.
    /// </summary>
    bool IsNextSceneAvailable { get; }

    /// <summary>
    /// Whether the game is scheduled to restart after exiting it.
    /// </summary>
    bool IsRestartScheduled { get; }

    /// <summary>
    /// The event raised when the next scene finishes loading.
    /// </summary>
    event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;

    /// <summary>
    /// The event raised when the scene which comes next is changed.
    /// <para>Canceling this event prevents the scene next from switching.</para>
    /// </summary>
    event EventHandler<NextSceneChangeEventArgs>? NextSceneChange;

    /// <summary>
    /// The event raised when the currently active scene is switched.
    /// <para>Canceling this event prevents the active scene from switching.</para>
    /// </summary>
    event EventHandler<SceneChangeEventArgs>? ActiveSceneChange;


    // Methods.
    /// <summary>
    /// Schedules the next scene to be set to the given scene. This may instantly set the scene
    /// on the call to this method or delay setting it to a later time when possible.
    /// <para>This method should automatically load the next scene.</para>
    /// </summary>
    /// <param name="scene">The scene to set as the next scene.</param>
    void ScheduleNextSceneSet(IGameScene? scene);

    /// <summary>
    /// Schedules the scene to be changed to the scene which is set as the next one.
    /// <para>If the next scene is currently not set or hasn't been loaded yet, this should not do anything.</para>
    /// </summary>
    /// <param name="shouldJump">Whether the scene should be changed.</param>
    void ScheduleJumpToNextScene(bool shouldJump);

    /// <summary>
    /// Ends the current scene, unloads it and then exits the game.
    /// </summary>
    void Exit();

    /// <summary>
    /// Ends the current scene, unloads it and then restarts the game.
    /// </summary>
    void Restart();
}