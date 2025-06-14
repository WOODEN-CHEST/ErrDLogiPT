using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

public class SceneComponentEventArgs : SceneEventArgs
{
    // Fields.
    public ISceneComponent? Component { get; set; }


    // Constructors.
    public SceneComponentEventArgs(IGameScene scene, ISceneComponent? component) : base(scene)
    {
        Component = component;
    }
}