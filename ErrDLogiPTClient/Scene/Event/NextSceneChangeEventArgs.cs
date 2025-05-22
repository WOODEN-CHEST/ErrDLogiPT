using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

public class NextSceneChangeEventArgs : CancellableEventBase
{
    // Fields.
    public IGameScene? CurrentScene { get; private init; }
    public IGameScene? NextScene { get; private init; }


    // Constructors.
    public NextSceneChangeEventArgs(IGameScene? currentScene, IGameScene? nextScene)
    {
        CurrentScene = currentScene;
        NextScene = nextScene;
    }
}