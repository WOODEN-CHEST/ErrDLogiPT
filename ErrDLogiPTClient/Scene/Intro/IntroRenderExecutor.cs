using ErrDLogiPTClient.Service;
using GHEngine.Frame;
using System;

namespace ErrDLogiPTClient.Scene.Intro;

/// <summary>
/// Handles rendering for the intro scene.
/// </summary>
public class IntroRenderExecutor : SceneComponentBase
{
    // Fields.

    /// <summary>
    /// Whether the text "Loading..." is shown after the logo had finished displaying and while the game is still loading.
    /// </summary>
    public virtual bool IsLoadingShown
    {
        get => _isLoadingShown;
        set
        {
            _isLoadingShown = value;
            _loadingDisplayer.IsDisplayed = value;
        }
    }

    /// <summary>
    /// Whether the logo animation has been completed.
    /// </summary>
    public virtual bool IsAnimationDone { get; protected set; } = false;


    /// <summary>
    /// Event raised when the logo animation is completed. Skipping the animation counts as completing it, thus
    /// also raises this event.
    /// </summary>
    public event EventHandler<EventArgs>? LogoAnimationDone;


    // Protected fields.

    /// <summary>
    /// Name of the render layer which contains the logo sprite, used to fetch that layer from the game frame.
    /// </summary>
    protected virtual string LogoLayerName
    {
        get => _logoLayerName;
        set => _logoLayerName = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The render frame which is used in rendering this scene.
    /// <para>Note that since the render frame is set to be the current frame in the <see cref="IFrameRenderer"/>
    /// object only when the scene starts, changing this after the scene has started will do nothing.</para>
    /// </summary>
    protected virtual IGameFrame Frame
    {
        get => _frame;
        set => _frame = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The object use to detect the user skipping the intro logo animation.
    /// </summary>
    protected IntroSkipper IntroSkipper => _introSkipper;

    /// <summary>
    /// The object used to display the "Loading..." text to the user after the logo animation has been finished.
    /// </summary>
    protected IntroLoadingDisplayer LoadingDisplayer => _loadingDisplayer;

    /// <summary>
    /// The object used to display the intro logo animation.
    /// </summary>
    protected IntroLogoDisplayer LogoDisplayer => _logoDisplayer;


    // Private fields.
    private IntroLogoDisplayer _logoDisplayer;
    private IntroLoadingDisplayer _loadingDisplayer;
    private IntroSkipper _introSkipper;
    private IGameFrame _frame;
    private string _logoLayerName = "logo";

    private bool _isLoadingShown = true;


    // Constructors.
    public IntroRenderExecutor(IntroScene scene, IGenericServices services) : base(scene, services) { }


    // Protected methods.

    /// <summary>
    /// Creates an instance of a logo displayer.
    /// </summary>
    /// <para>This method can be overridden in a derived class to provide a custom implementation of
    /// <see cref="IntroLogoDisplayer"/>.</para>
    protected virtual IntroLogoDisplayer CreateLogoDisplayer()
    {
        return new IntroLogoDisplayer(Scene, SceneServices, Frame.GetLayer(LogoLayerName)!);
    }

    /// <summary>
    /// Creates an instance of a loading text displayer.
    /// </summary>
    /// <para>This method can be overridden in a derived class to provide a custom implementation of
    /// <see cref="IntroLoadingDisplayer"/>.</para>
    protected virtual IntroLoadingDisplayer CreateLoadingDisplayer()
    {
        return new IntroLoadingDisplayer(Scene, SceneServices, Frame.GetLayer(LogoLayerName)!);
    }

    /// <summary>
    /// Creates an instance of an intro skipper.
    /// </summary>
    /// <para>This method can be overridden in a derived class to provide a custom implementation of
    /// <see cref="IntroSkipper"/>.</para>
    protected virtual IntroSkipper CreateIntroSkipper()
    {
        return new IntroSkipper(Scene, SceneServices);
    }

    /// <summary>
    /// Invokes the <see cref="LogoAnimationDone"/> event.
    /// </summary>
    protected void InvokeLogoAnimationDoneEvent()
    {
        LogoAnimationDone?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called specifically when the intro logo animation is skipped by the user.
    /// </summary>
    protected virtual void HandleIntroSkip()
    {
        _logoDisplayer.SkipAnimation();
        OnAnimationComplete();
    }

    /// <summary>
    /// Called specifically when the intro logo animation ends naturally (not skipped).
    /// </summary>
    protected virtual void HandleLogoShowFinish()
    {
        OnAnimationComplete();
    }

    /// <summary>
    /// Called when the logo animation either completes or is skipped.
    /// </summary>
    protected virtual void OnAnimationComplete()
    {
        _loadingDisplayer.IsDisplayed = IsLoadingShown;
        IsAnimationDone = true;
        InvokeLogoAnimationDoneEvent();
    }


    // Private methods.
    private void OnIntroSkipEvent(object? sender, EventArgs args)
    {
        HandleIntroSkip();
    }

    private void OnLogoShowFinishEvent(object? sender, EventArgs args)
    {
        HandleLogoShowFinish();
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        Frame = new GHGameFrame();

        ILayer TargetLayer = new GHLayer(LogoLayerName);
        Frame.AddLayer(TargetLayer);

        _logoDisplayer = CreateLogoDisplayer();
        _logoDisplayer.LogoShowFinish += OnLogoShowFinishEvent;

        _loadingDisplayer = CreateLoadingDisplayer();

        _introSkipper = CreateIntroSkipper();
        _introSkipper.SkippedIntro += OnIntroSkipEvent;

        AddComponent(_logoDisplayer);
        AddComponent(_loadingDisplayer);
        AddComponent(_introSkipper);
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPreComponent();

        SceneServices.GetRequired<IFrameExecutor>().SetFrame(_frame);
    }
}