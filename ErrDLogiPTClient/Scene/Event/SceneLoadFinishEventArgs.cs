using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

public class SceneLoadFinishEventArgs : SceneEventArgs
{
    public SceneLoadFinishEventArgs(IGameScene scene) : base(scene)
    {
    }
}