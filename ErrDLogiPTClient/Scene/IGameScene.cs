using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface IGameScene : ITimeUpdatable
{
    // Fields.
    event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;
    bool IsLoaded { get; }

    // Methods.
    void Load();
    void OnStart();
    void OnEnd();
    void Unload();
}