using ErrDLogiPTClient.Scene;
using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Service;
using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrappedServiceSceneExecutor : ServiceWrapper<ISceneExecutor>, ISceneExecutor
{
    // Fields.
    public IGameScene? CurrentScene => throw new NotImplementedException();
    public bool IsNextSceneLoaded => throw new NotImplementedException();
    public bool IsNextSceneAvailable => throw new NotImplementedException();
    public bool IsRestartScheduled => throw new NotImplementedException();

    public event EventHandler<SceneLoadFinishEventArgs>? SceneLoadFinish;
    public event EventHandler<NextSceneChangeEventArgs>? NextSceneChange;
    public event EventHandler<SceneChangeEventArgs>? ActiveSceneChange;


    // Constructors.
    public WrappedServiceSceneExecutor(IGenericServices services) : base(services) { }


    // Private methods.
    private void OnSceneLoadFinishEvent(object? sender, SceneLoadFinishEventArgs args)
    {
        SceneLoadFinish?.Invoke(this, args);
    }

    private void OnNextSceneChangeEvent(object? sender, NextSceneChangeEventArgs args)
    {
        NextSceneChange?.Invoke(this, args);
    }

    private void OnActiveSceneChangeEvent(object? sender, SceneChangeEventArgs args)
    {
        ActiveSceneChange?.Invoke(this, args);
    }


    // Inherited methods.
    public void Exit()
    {
        ServiceObject.Exit();
    }

    public void Restart()
    {
        ServiceObject.Restart();
    }

    public void ScheduleJumpToNextScene(bool shouldJump)
    {
        ServiceObject.ScheduleJumpToNextScene(shouldJump);
    }

    public void ScheduleNextSceneSet(IGameScene? scene)
    {
        ServiceObject.ScheduleNextSceneSet(scene);
    }

    public void Update(IProgramTime time) { }

    protected override void InitService(ISceneExecutor service)
    {
        service.SceneLoadFinish += OnSceneLoadFinishEvent;
        service.NextSceneChange += OnNextSceneChangeEvent;
        service.ActiveSceneChange += OnActiveSceneChangeEvent;
    }

    protected override void DeinitService(ISceneExecutor service)
    {
        service.SceneLoadFinish -= OnSceneLoadFinishEvent;
        service.NextSceneChange -= OnNextSceneChangeEvent;
        service.ActiveSceneChange -= OnActiveSceneChangeEvent;
    }
}