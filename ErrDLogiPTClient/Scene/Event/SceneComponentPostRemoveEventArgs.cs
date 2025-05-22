using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

public class SceneComponentPostRemoveEventArgs : SceneComponentEventArgs
{
    // Constructors.
    public SceneComponentPostRemoveEventArgs(IGameScene scene, ISceneComponent component)
        : base(scene, component) { }
}