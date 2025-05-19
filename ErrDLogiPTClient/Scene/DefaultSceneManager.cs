using GHEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public class DefaultSceneManager : ISceneManager
{
    // Fields.
    public IGameScene? CurrentScene { get; private set; }

    public bool IsNextSceneLoaded => throw new NotImplementedException();

    public bool IsNextSceneAvailable => throw new NotImplementedException();

    public event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;
    public event EventHandler<NextSceneChangeEventArgs>? NextSceneChange;


    // Private fields.
    private IGameScene? _nextScene = null;
    private bool _isSceneScheduledToJump = false;
    private readonly ConcurrentQueue<Action> _scheduledActions = new();



    // Private methods.
    private void DisposeScene(IGameScene scene)
    {

    }

    private void JumpToNextScene()
    {
        IGameScene? OldScene = CurrentScene;
        CurrentScene = _nextScene;
        _nextScene = null;

        if (OldScene != null)
        {
            DisposeScene(OldScene);
        }
    }


    // Inherited methods.
    public bool ScheduleJumpToNextScene()
    {
        if ((_nextScene == null) || !IsNextSceneLoaded)
        {
            return false;
        }

        _isSceneScheduledToJump = true;
        return true;
    }

    public void SetNextScene(IGameScene? scene)
    {
        if (_nextScene != null)
        {

        }

        _nextScene = scene;
    }

    public void Update(IProgramTime time)
    {
        if (_isSceneScheduledToJump)
        {
            if (_nextScene !=  null)
            {
                JumpToNextScene();
            }
            _isSceneScheduledToJump = false;
        }
        else
        {
            CurrentScene?.Update(time);
        }
    }
}