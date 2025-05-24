using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

public class SceneComponentRemoveEventArgs : SceneComponentEventArgs
{
    // Constructors.
    public SceneComponentRemoveEventArgs(IGameScene scene, ISceneComponent component)
        : base(scene, component) { }
}