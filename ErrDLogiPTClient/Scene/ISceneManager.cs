using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface ISceneManager : ITimeUpdatable
{
    // Fields.
    public IGameScene? CurrentScene { get; }
    public bool IsNextSceneLoaded { get; }
    public bool IsNextSceneAvailable { get; }

    event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;
    event EventHandler<NextSceneChangeEventArgs>? NextSceneChange;
    event EventHandler<SceneChangeEventArgs>? ActiveSceneChange;


    // Methods.
    void SetNextScene(IGameScene? scene);
    void ScheduleJumpToNextScene(bool shouldJump);
}