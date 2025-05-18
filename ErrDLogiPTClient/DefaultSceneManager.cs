using ErrDLogiPTClient.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public class DefaultSceneManager : ISceneManager
{
    public IGameScene? CurrentScene => throw new NotImplementedException();

    public bool IsNextSceneLoaded => throw new NotImplementedException();

    public bool IsNextSceneAvailable => throw new NotImplementedException();

    public event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;

    public bool ScheduleJumpToNextScene()
    {
        throw new NotImplementedException();
    }

    public void SetNextScene(IGameScene? scene)
    {
        throw new NotImplementedException();
    }
}