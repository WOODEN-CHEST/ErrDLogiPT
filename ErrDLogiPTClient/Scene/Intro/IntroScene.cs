using ErrDLogiPTClient.Mod;
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
    private readonly IModManager _modManager;
    private readonly ILogiAssetManager _logiAssetManager;

    private readonly IGameScene _mainMenuScene;
    
    private readonly IntroFrameExecutor _frameExecutor;


    // Constructors.
    public IntroScene(IGameServices services, IModManager modManager) : base(services)
    {
        _modManager = modManager ?? throw new ArgumentNullException(nameof(modManager));
        _logiAssetManager = new LogiAssetManager(Services, modManager);
        _frameExecutor = new(this, AssetProvider, Services);
        _frameExecutor.AnimationDone += OnAnimationFinishEvent;

        Components.Add(_frameExecutor);
        Components.Add(new GamePropertiesInitializer(this, AssetProvider, Services));
        Components.Add(new GameHotkeyExecutor(this, AssetProvider, Services));

        _mainMenuScene = new MainMenuScene(Services, modManager, _logiAssetManager);
    }


    // Private methods.
    private void OnNextSceneLoadEvent(object? sender, SceneLoadFinishEventArgs args)
    {
        if ((_mainMenuScene == args.Scene) && _frameExecutor.IsAnimationDone)
        {
            Services.SceneManager.ScheduleJumpToNextScene(true);
        }
        _frameExecutor.IsLoadingShown = false;
    }

    private void OnAnimationFinishEvent(object? sender, EventArgs args)
    {
        if (_mainMenuScene.IsLoaded)
        {
            Services.SceneManager.ScheduleJumpToNextScene(true);
        }
    }


    // Inherited methods.
    protected override void HandleLoad()
    {
        base.HandleLoad();

        _logiAssetManager.LoadAssetDefinitions();
        _logiAssetManager.SetAsserRootPaths(Array.Empty<string>());
    }

    public override void OnStart()
    {
        base.OnStart();

        Services.SceneManager.SceneLoadFinish += OnNextSceneLoadEvent;
        Services.SceneManager.SetNextScene(_mainMenuScene);
    }
}