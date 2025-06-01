using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Scene.MainMenu;
using GHEngine;
using GHEngine.Assets.Loader;
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
    private readonly IntroRenderExecutor _renderExecutor;
    private readonly IntroSkipper _introSkipper;


    // Constructors.
    public IntroScene(GenericServices services) : base(services)
    {
        _renderExecutor = new(this, SceneServices);
        _renderExecutor.AnimationDone += OnAnimationFinishEvent;

        AddComponent(_renderExecutor);
        AddComponent(new GamePropertiesInitializer(this, SceneServices));

        _introSkipper = new(this, SceneServices);
        AddComponent(_introSkipper);

        _nextSceneMainMenu = new MainMenuScene(GlobalServices);
    }


    // Private methods.
    private void OnNextSceneLoadEvent(object? sender, SceneLoadFinishEventArgs args)
    {
        if ((_nextSceneMainMenu == args.Scene) && _renderExecutor.IsAnimationDone)
        {
            SceneServices.GetRequired<ISceneExecutor>().ScheduleJumpToNextScene(true);
        }
        _renderExecutor.IsLoadingShown = false;
    }

    private void OnAnimationFinishEvent(object? sender, EventArgs args)
    {
        if (_nextSceneMainMenu.LoadStatus == SceneLoadStatus.FinishedLoading)
        {
            SceneServices.GetRequired<ISceneExecutor>().ScheduleJumpToNextScene(true);
        }
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        IModManager ModManager = SceneServices.GetRequired<IModManager>();
        ILogiAssetLoader AssetLoader = SceneServices.GetRequired<ILogiAssetLoader>();
        IGamePathStructure Structure = SceneServices.GetRequired<IGamePathStructure>();

        ModManager.LoadMods(Structure.ModRoot);
        AssetLoader.SetAssetRootPaths(Array.Empty<string>());
        AssetLoader.LoadAssetDefinitions();
        ModManager.InitializeMods(SceneServices);
    }

    public override void OnStart()
    {
        base.OnStart();

        ISceneExecutor SceneExecutor = SceneServices.GetRequired<ISceneExecutor>();
        SceneExecutor.SceneLoadFinish += OnNextSceneLoadEvent;
        SceneExecutor.ScheduleNextSceneSet(_nextSceneMainMenu);
    }
}