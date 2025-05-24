using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

public class SubComponentEventArgs : CancellableEventBase
{
    // Fields.
    public IGameScene Scene => Component.Scene;
    public ISceneComponent Component { get; private init; }
    public ISceneComponent SubComponent { get; private init; }


    // Constructors.
    public SubComponentEventArgs(ISceneComponent component, ISceneComponent subComponent)
    {
        Component = component ?? throw new ArgumentNullException(nameof(component));
        SubComponent = subComponent ?? throw new ArgumentNullException(nameof(subComponent));
    }
}