using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Event;

public class SubComponentRemoveEventArgs : SubComponentEventArgs
{
    public SubComponentRemoveEventArgs(ISceneComponent component, ISceneComponent subComponent)
        : base(component, subComponent) { }
}