using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

public class SceneEventArgs : CancellableEventBase
{
    // Fields.
    public IGameScene Scene { get; private init; }


    // Constructors.
    public SceneEventArgs(IGameScene scene)
    {
        Scene = scene ?? throw new ArgumentNullException(nameof(scene));
    }
}