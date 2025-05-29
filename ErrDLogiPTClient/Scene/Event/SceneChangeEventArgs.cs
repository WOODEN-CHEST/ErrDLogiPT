using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

/// <summary>
/// Event which is raised when the currently active scene is changed by a scene executor.
/// <para>If this event is canceled, then the active scene doesn't switch.</para>
/// </summary>
public class SceneChangeEventArgs : CancellableEventBase
{
    // Fields.

    /// <summary>
    /// The scene which was active before this switch happened.
    /// </summary>
    public IGameScene? OldScene { get; private init; }

    /// <summary>
    /// The new scene which will be active after this switch happens.
    /// </summary>
    public IGameScene? NewScene { get; private init; }


    // Constructors.
    public SceneChangeEventArgs(IGameScene? oldScene, IGameScene? newScene)
    {
        OldScene = oldScene;
        NewScene = newScene;
    }
}