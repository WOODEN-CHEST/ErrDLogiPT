using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public class SceneChangeEventArgs : EventArgs
{
    // Fields.
    public IGameScene? OldScene { get; private init; }
    public IGameScene? NewScene { get; private init; }


    // Constructors.
    public SceneChangeEventArgs(IGameScene? oldScene, IGameScene? newScene)
    {
        OldScene = oldScene;
        NewScene = newScene;
    }
}