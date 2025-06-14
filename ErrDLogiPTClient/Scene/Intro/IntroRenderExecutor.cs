using GHEngine;
using GHEngine.Assets;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

public class IntroRenderExecutor : SceneComponentBase
{
    // Fields.
    public bool IsLoadingShown { get; set; } = true;
    public bool IsAnimationDone { get; private set; } = false;

    public event EventHandler<EventArgs>? LogoAnimationDone;


    // Protected fields.
    protected string LogoLayerName
    {
        get => _logoLayerName;
        set => _logoLayerName = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected IGameFrame Frame { get; }



    // Private fields.
    private IntroLogoDisplayer _logoDisplayer;
    private IntroLoadingDisplayer _loadingDisplayer;
    private IntroSkipper _introSkipper;
    private IGameFrame _frame;
    protected string _logoLayerName = "main";


    // Constructors.
    public IntroRenderExecutor(IntroScene scene, GenericServices services) : base(scene, services) { }


    // Private methods.
    private void OnIntroSkipEvent(object? sender, EventArgs args)
    {
        _logoDisplayer.SkipAnimation();
        OnAnimationComplete();
    }

    private void OnLogoShowFinishEvent(object? sender, EventArgs args)
    {
        OnAnimationComplete();
    }

    private void OnAnimationComplete()
    {
        _loadingDisplayer.IsDisplayed = IsLoadingShown;
        IsAnimationDone = true;
        LogoAnimationDone?.Invoke(this, EventArgs.Empty);
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        _frame = new GHGameFrame();

        ILayer TargetLayer = new GHLayer(LogoLayerName);
        _frame.AddLayer(TargetLayer);

        _logoDisplayer = new(TypedScene, SceneServices, TargetLayer);
        _logoDisplayer.LogoShowFinish += OnLogoShowFinishEvent;

        _loadingDisplayer = new(TypedScene, SceneServices, TargetLayer);

        _introSkipper = new(TypedScene, SceneServices);
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