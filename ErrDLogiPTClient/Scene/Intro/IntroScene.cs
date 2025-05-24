using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Scene.MainMenu;
using GHEngine;
using GHEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

public class IntroScene : SceneBase
{
    // Private fields.
    private readonly IGameScene _nextSceneMainMenu;
    private readonly IntroRenderExecutor _frameExecutor;
    private readonly IntroSkipper _introSkipper;


    // Constructors.
    public IntroScene(GameServices services) : base(services)
    {
        _frameExecutor = new(this, AssetProvider, Services.Input, services.FrameExecutor);
        _frameExecutor.AnimationDone += OnAnimationFinishEvent;

        AddComponent(_frameExecutor);
        AddComponent(new GamePropertiesInitializer(this, Services.Display, Services.Input));

        _introSkipper = new(this, services.Input);
        AddComponent(_introSkipper);

        _nextSceneMainMenu = new MainMenuScene(Services);
    }


    // Private methods.
    private void OnNextSceneLoadEvent(object? sender, SceneLoadFinishEventArgs args)
    {
        if ((_nextSceneMainMenu == args.Scene) && _frameExecutor.IsAnimationDone)
        {
            Services.SceneExecutor.ScheduleJumpToNextScene(true);
        }
        _frameExecutor.IsLoadingShown = false;
    }

    private void OnAnimationFinishEvent(object? sender, EventArgs args)
    {
        if (_nextSceneMainMenu.LoadStatus == SceneLoadStatus.FinishedLoading)
        {
            Services.SceneExecutor.ScheduleJumpToNextScene(true);
        }
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        Services.ModManager.LoadMods(Services.Structure.ModRoot);
        Services.ModManager.InitializeMods(Services);
        Services.AssetManager.SetAssetRootPaths(Array.Empty<string>());
        Services.AssetManager.LoadAssetDefinitions();
    }

    public override void OnStart()
    {
        base.OnStart();

        Services.SceneExecutor.SceneLoadFinish += OnNextSceneLoadEvent;
        Services.SceneExecutor.ScheduleNextSceneSet(_nextSceneMainMenu);
    }
}