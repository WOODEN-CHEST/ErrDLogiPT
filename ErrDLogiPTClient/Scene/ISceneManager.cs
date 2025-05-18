using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public interface ISceneManager
{
    // Fields.
    public IGameScene? CurrentScene { get; }
    public bool IsNextSceneLoaded { get; }
    public bool IsNextSceneAvailable { get; }

    event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;


    // Methods.
    void SetNextScene(IGameScene? scene);
    bool ScheduleJumpToNextScene();
}