using ErrDLogiPTClient.Scene.Event;
using GHEngine;
using GHEngine.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

public class DefaultSceneExecutor : ISceneExecutor
{
    // Fields.
    public IGameScene? CurrentScene
    {
        get
        {
            lock (_lockObject)
            {
                return _currentScene;
            }
        }
    }

    public bool IsNextSceneLoaded
    {
        get
        {
            lock (_lockObject)
            {
                return _isNextSceneLoaded;
            }
        }
    }

    public bool IsNextSceneAvailable
    {
        get
        {
            lock (_lockObject)
            {
                return _isNextSceneAvailable;
            }
        }
    }

    public event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;
    public event EventHandler<NextSceneChangeEventArgs>? NextSceneChange;
    public event EventHandler<SceneChangeEventArgs>? ActiveSceneChange;


    // Private fields.
    private readonly object _lockObject = new();
    private readonly ILogger? _logger;
    private readonly ConcurrentQueue<Action> _scheduledActions = new();

    private IGameScene? _nextScene = null;
    private bool _isSceneScheduledToJump = false;
    private IGameScene? _currentScene = null;
    private bool _isNextSceneLoaded = false;
    private bool _isNextSceneAvailable = false;


    // Constructors.
    public DefaultSceneExecutor(ILogger? logger)
    {
        _logger = logger;
    }


    // Private methods.
    private void ExecuteScheduledActions()
    {
        while (_scheduledActions.TryDequeue(out Action? TargetAction))
        {
            TargetAction.Invoke();
        }
    }

    private void DisposeScene(IGameScene scene)
    {
        try
        {
            scene.Unload();
        }
        catch (Exception e)
        {
            _logger?.Error($"Unhandled exception while unloading scene: {e}");
        }
    }

    private void LoadNextScene(IGameScene scene)
    {
        try
        {
            scene.Load();
            lock (_lockObject)
            {
                if (_nextScene != scene)
                {
                    DisposeScene(scene);
                    return;
                }

                _isNextSceneLoaded = true;
                _scheduledActions.Enqueue(() => SceneLoadFinish?.Invoke(this, new(scene)));
            }
        }
        catch (Exception e)
        {
            _scheduledActions.Enqueue(() => throw new Exception($"Unhandled exception while loading next scene: {e}"));
        }
    }

    private bool TrySwitchScene()
    {
        IGameScene? OldScene, NewScene;
        lock (_lockObject)
        {
            if (!(_isNextSceneAvailable && _isNextSceneLoaded && _isSceneScheduledToJump))
            {
                return false;
            }

            OldScene = _currentScene;
            NewScene = _nextScene;

            _currentScene = NewScene;
            _nextScene = null;

            _isNextSceneLoaded = false;
            _isSceneScheduledToJump = false;
        }

        if (OldScene != null)
        {
            OldScene.OnEnd();
            Task.Run(() => DisposeScene(OldScene));
        }

        NewScene?.OnStart();

        ActiveSceneChange?.Invoke(this, new(OldScene, NewScene));
        return true;
    }


    // Inherited methods.
    public void ScheduleJumpToNextScene(bool shouldJump)
    {
        lock (_lockObject)
        {
            _isSceneScheduledToJump = shouldJump;
        }
    }

    public void SetNextScene(IGameScene? nextScene)
    {
        IGameScene? CurrentlyActiveScene;

        lock (_lockObject)
        {
            if (nextScene == _nextScene)
            {
                return;
            }

            _isNextSceneLoaded = false;
            _nextScene = nextScene;

            _isNextSceneAvailable = nextScene != null;
            if (_isNextSceneAvailable)
            {
                Task.Run(() => LoadNextScene(nextScene!));
            }
            CurrentlyActiveScene = _currentScene;
        }

        NextSceneChange?.Invoke(this, new(CurrentlyActiveScene, nextScene));
    }

    public void Update(IProgramTime time)
    {
        ExecuteScheduledActions();

        bool WasSceneSwitched = TrySwitchScene();

        if (!WasSceneSwitched)
        {
            CurrentScene?.Update(time);
        }
    }
}