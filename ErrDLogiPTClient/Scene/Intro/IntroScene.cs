using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.OS;
using ErrDLogiPTClient.OS.Logi.LogiXD;
using ErrDLogiPTClient.Registry;
using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Scene.MainMenu;
using ErrDLogiPTClient.Service;
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
    private IGameScene _nextSceneMainMenu;
    private IntroRenderExecutor _renderExecutor;


    // Constructors.
    public IntroScene(GlobalServices services) : base(services) { }


    // Protected methods.
    protected virtual IntroRenderExecutor GetIntroRenderExecutor()
    {
        return new IntroRenderExecutor(this, SceneServices);
    }

    protected void OnAnimationFinishEvent(object? sender, EventArgs args)
    {
        if (_nextSceneMainMenu.LoadStatus == SceneLoadStatus.FinishedLoading)
        {
            OnReadyForNextScene();
        }
    }

    protected virtual void OnReadyForNextScene()
    {
        SceneServices.GetRequired<ISceneExecutor>().ScheduleJumpToNextScene(true);
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

    private void CreateRegistryStorage()
    {
        IGameRegistryStorage RegistryStorage = new DefaultRegistryStorage();
        RegistryStorage.SetRegistry<IGameOSDefinition>(GetOSRegistry());
        GlobalServices.Set<IGameRegistryStorage>(RegistryStorage);
    }

    private IGameItemRegistry<IGameOSDefinition> GetOSRegistry()
    {
        IGameItemRegistry<IGameOSDefinition> Registry = new DefaultItemRegistry<IGameOSDefinition>();

        IGameOSDefinition LogiXDDefinition = new LogiXDDefinition();
        Registry.Register(LogiXDDefinition.DefinitionKey, LogiXDDefinition);

        return Registry;
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        _renderExecutor = GetIntroRenderExecutor();
        _renderExecutor.LogoAnimationDone += OnAnimationFinishEvent;

        IModManager ModManager = SceneServices.GetRequired<IModManager>();
        ILogiAssetManager AssetLoader = SceneServices.GetRequired<ILogiAssetManager>();
        IGamePathStructure Structure = SceneServices.GetRequired<IGamePathStructure>();
        CreateRegistryStorage();

        ModManager.LoadMods(Structure.ModRoot);
        AssetLoader.SetAssetRootPaths(Array.Empty<string>());
        AssetLoader.LoadAssetDefinitions();
        ModManager.InitializeMods(SceneServices);

        AddComponent(_renderExecutor);
        AddComponent(new GamePropertiesInitializer(this, SceneServices));

        _nextSceneMainMenu = new MainMenuScene(GlobalServices);
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPreComponent();

        ISceneExecutor SceneExecutor = SceneServices.GetRequired<ISceneExecutor>();
        SceneExecutor.SceneLoadFinish += OnNextSceneLoadEvent;
        SceneExecutor.ScheduleNextSceneSet(_nextSceneMainMenu);
    }
}