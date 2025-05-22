using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

public class SceneComponentPreAddEventArgs : SceneComponentEventArgs
{
    // Fields.
    public bool IsCancelled { get; set; } = false;


    // Constructors.
    public SceneComponentPreAddEventArgs(IGameScene scene, ISceneComponent component)
        : base(scene, component) { }
}