using ErrDLogiPTClient.Scene.Event;
using ErrDLogiPTClient.Scene.MainMenu;
using ErrDLogiPTClient.Service;
using System;

namespace ErrDLogiPTClient.Scene.Intro;

/// <summary>
/// The intro scene simply shows the logo to the player and then shows the loading screen
/// while the main menu scene is being loaded. 
/// </summary>
public class IntroScene : SceneBase
{
    // Private fields.
    private IGameScene _nextSceneMainMenu;
    private IntroRenderExecutor _renderExecutor;


    // Constructors.
    public IntroScene(IGenericServices services) : base(services) { }


    // Protected methods.

    /// <summary>
    /// Gets the object used for rendering the intro scene.
    /// <para>This can be overridden to supply a custom intro render executor.</para>
    /// </summary>
    protected virtual IntroRenderExecutor GetIntroRenderExecutor()
    {
        return new IntroRenderExecutor(this, SceneServices);
    }

    /// <summary>
    /// Method called when the logo display animation finishes.
    /// </summary>
    protected virtual void OnAnimationFinishEvent(object? sender, EventArgs args)
    {
        if (_nextSceneMainMenu.LoadStatus == SceneLoadStatus.FinishedLoading)
        {
            OnReadyForNextScene();
        }
    }

    /// <summary>
    /// Method called when the intro animation has been finished and the next scene has already loaded
    /// and is ready to be switched to.
    /// </summary>
    protected virtual void OnReadyForNextScene()
    {
        SceneServices.GetRequired<ISceneExecutor>().ScheduleJumpToNextScene(true);
    }

    /// <summary>
    /// Gets the next scene that will be set as the next scene after this one (defaults to <see cref="MainMenuScene"/>).
    /// <para>This can be overridden to supply a custom main menu scene.</para>
    /// </summary>
    protected virtual IGameScene CreateNextScene()
    {
        return new MainMenuScene(GlobalGameServices);
    }


    // Private methods.
    private void OnNextSceneLoadEvent(object? sender, SceneLoadFinishEventArgs args)
    {
        if ((_nextSceneMainMenu == args.Scene) && _renderExecutor.IsAnimationDone)
        {
            OnReadyForNextScene();
        }
        _renderExecutor.IsLoadingShown = false;
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        _renderExecutor = GetIntroRenderExecutor();
        _renderExecutor.LogoAnimationDone += OnAnimationFinishEvent;
        AddComponent(_renderExecutor);

        _nextSceneMainMenu = CreateNextScene();
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPreComponent();

        ISceneExecutor SceneExecutor = SceneServices.GetRequired<ISceneExecutor>();
        SceneExecutor.SceneLoadFinish += OnNextSceneLoadEvent;
        SceneExecutor.ScheduleNextSceneSet(_nextSceneMainMenu);
    }

    protected override void HandleEndPreComponent()
    {
        base.HandleEndPreComponent();

        SceneServices.GetRequired<ISceneExecutor>().SceneLoadFinish -= OnNextSceneLoadEvent;
    }
}