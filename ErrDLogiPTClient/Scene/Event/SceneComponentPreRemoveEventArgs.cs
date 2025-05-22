using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

public class SceneComponentPreRemoveEventArgs : SceneComponentEventArgs
{
    // Fields.
    public bool IsCancelled { get; set; } = false;


    // Constructors.
    public SceneComponentPreRemoveEventArgs(IGameScene scene, ISceneComponent component)
        : base(scene, component) { }
}