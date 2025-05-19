using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public class SceneLoadFinishEventArgs : EventArgs
{
    // Fields.
    public IGameScene Scene { get; private init; }


    // Constructors.
    public SceneLoadFinishEventArgs(IGameScene scene)
    {
        Scene = scene ?? throw new ArgumentNullException(nameof(scene));
    }
}