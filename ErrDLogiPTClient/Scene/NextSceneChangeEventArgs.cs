using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public class NextSceneChangeEventArgs : EventArgs
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