﻿using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Service;
using GHEngine;
using GHEngine.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene;

/// <summary>
/// The default scene executor.
/// <para>Event success actions for this implementation are ran <b>after</b> performing the event related work.</para>
/// <para>Setting the next scene schedules it to be set on the next
/// time this executor is updated via call to <c>Update()</c></para>
/// <para>If a jump to the next scene is scheduled, it also is executed on the next update call.</para>
/// </summary>
public class DefaultSceneExecutor : ISceneExecutor
{
    // Fields.
    public IGameScene? CurrentScene
    {
        get
        {
            lock (LockObject)
            {
                return _currentScene;
            }
        }
    }

    public bool IsNextSceneLoaded
    {
        get
        {
            lock (LockObject)
            {
                return _isNextSceneLoaded;
            }
        }
    }

    public bool IsNextSceneAvailable
    {
        get
        {
            lock (LockObject)
            {
                return _isNextSceneAvailable;
            }
        }
    }

    public bool IsRestartScheduled { get; private set; }

    public event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;
    public event EventHandler<NextSceneChangeEventArgs>? NextSceneChange;
    public event EventHandler<SceneChangeEventArgs>? ActiveSceneChange;


    // Protected fields.
    protected object LockObject { get; } = new();
    protected IGenericServices GlobalServices { get; private init; }


    // Private fields.
    private readonly ConcurrentQueue<Action> _scheduledActions = new();
    private IGameScene? _nextScene = null;
    private bool _isSceneScheduledToJump = false;
    private IGameScene? _currentScene = null;
    private bool _isNextSceneLoaded = false;
    private bool _isNextSceneAvailable = false;
    private readonly Game _game;


    // Constructors.
    public DefaultSceneExecutor(Game game, IGenericServices services)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        GlobalServices = services ?? throw new ArgumentNullException(nameof(services));
    }


    // Protected methods.
    protected void EnqueueScheduledAction(Action action)
    {
        _scheduledActions.Enqueue(action);
    }

    protected void ExecuteScheduledActions()
    {
        while (_scheduledActions.TryDequeue(out Action? TargetAction))
        {
            TargetAction.Invoke();
        }
    }

    protected void ClearScheduledActions()
    {
        _scheduledActions.Clear();
    }


    // Private methods.
    private void DisposeScene(IGameScene scene)
    {
        try
        {
            scene.Unload();
        }
        catch (Exception e)
        {
            GlobalServices.GetRequired<ILogger>().Error($"Unhandled exception while unloading scene: {e}");
        }
    }

    private void LoadNextScene(IGameScene scene)
    {
        try
        {
            scene.Load();
            bool IsCorrectSceneLoaded;
            lock (LockObject)
            {
                IsCorrectSceneLoaded = _nextScene == scene;
                _isNextSceneLoaded = IsCorrectSceneLoaded;
            }

            if (IsCorrectSceneLoaded)
            {
                EnqueueScheduledAction(() => SceneLoadFinish?.Invoke(this, new(scene)));
            }
            else
            {
                DisposeScene(scene);
            }
        }
        catch (Exception e)
        {
            EnqueueScheduledAction(() => throw new Exception($"Unhandled exception while loading next scene: {e}"));
        }
    }

    private bool TrySwitchScene()
    {
        IGameScene? OldScene, NewScene;

        OldScene = _currentScene;
        NewScene = _nextScene;

        SceneChangeEventArgs ChangeEventArgs = new(OldScene, NewScene);
        ActiveSceneChange?.Invoke(this, ChangeEventArgs);

        if (ChangeEventArgs.IsCancelled)
        {
            ChangeEventArgs.ExecuteActions();
            return false;
        }

        lock (LockObject)
        {
            if (!(_isNextSceneAvailable && _isNextSceneLoaded && _isSceneScheduledToJump))
            {
                return false;
            }

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
        ChangeEventArgs.ExecuteActions();

        return true;
    }


    // Inherited methods.
    public virtual void ScheduleJumpToNextScene(bool shouldJump)
    {
        lock (LockObject)
        {
            _isSceneScheduledToJump = shouldJump;
        }
    }

    public virtual void ScheduleNextSceneSet(IGameScene? nextScene)
    {
        EnqueueScheduledAction(() =>
        {
            if (_currentScene == nextScene)
            {
                return;
            }

            NextSceneChangeEventArgs SceneChangeArgs = new(_currentScene, nextScene);
            NextSceneChange?.Invoke(this, SceneChangeArgs);
            
            if (SceneChangeArgs.IsCancelled)
            {
                SceneChangeArgs.ExecuteActions();
                return;
            }

            IGameScene? FinalNextScene = SceneChangeArgs.NextScene;
            _nextScene = FinalNextScene;

            lock (LockObject)
            {
                _isNextSceneLoaded = false;
                _isNextSceneAvailable = FinalNextScene != null;
                if (_isNextSceneAvailable)
                {
                    Task.Run(() => LoadNextScene(FinalNextScene!));
                }
            }

            SceneChangeArgs.ExecuteActions();
        });
    }

    public virtual void Update(IProgramTime time)
    {
        ExecuteScheduledActions();

        bool WasSceneSwitched = TrySwitchScene();

        if (!WasSceneSwitched)
        {
            CurrentScene?.Update(time);
        }
    }

    public void Exit()
    {
        IGameScene? SceneToEnd = CurrentScene;

        if (SceneToEnd != null)
        {
            SceneToEnd.OnEnd();
            SceneToEnd.Unload();
        }

        _game.Exit();
    }

    public void Restart()
    {
        IsRestartScheduled = true;
        Exit();
    }
}