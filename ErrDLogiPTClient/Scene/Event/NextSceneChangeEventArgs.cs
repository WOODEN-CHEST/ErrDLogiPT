using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;


/// <summary>
/// Event which is raised when the next scene that a scene manager should prepare is set.
/// <para>If this event is canceled, the next scene is not changed.</para>
/// </summary>
public class NextSceneChangeEventArgs : CancellableEventBase
{
    // Fields.

    /// <summary>
    /// The scene currently being executed by the scene executor, may be <c>null</c> for no scene.
    /// </summary>
    public IGameScene? CurrentScene { get; private init; }

    /// <summary>
    /// The next scene that the frame executor should prepare.
    /// <para>This property can be altered to change which scene should come next.</para>
    /// </summary>
    public IGameScene? NextScene { get; set; }


    // Constructors.
    public NextSceneChangeEventArgs(IGameScene? currentScene, IGameScene? nextScene)
    {
        CurrentScene = currentScene;
        NextScene = nextScene;
    }
}