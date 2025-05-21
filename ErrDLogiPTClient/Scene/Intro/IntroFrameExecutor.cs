using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

public class IntroFrameExecutor : SceneComponentBase<IntroScene>
{
    // Fields.
    public bool IsLoadingShown { get; set; } = true;
    public bool IsAnimationDone { get; private set; } = false;
    public event EventHandler<EventArgs>? AnimationDone;



    // Private static fields.
    private const string LOGO_LAYER_NAME = "main";


    // Private fields.
    private readonly IntroLogoDisplayer _logoDisplayer;
    private readonly IntroLoadingDisplayer _loadingDisplayer;
    private readonly IGameFrame _frame;


    // Constructors.
    public IntroFrameExecutor(IntroScene scene, 
        ISceneAssetProvider assetProvider,
        IGameServices services)
        : base(scene, assetProvider, services)
    {
        _frame = new GHGameFrame();

        ILayer TargetLayer = new GHLayer(LOGO_LAYER_NAME);
        _frame.AddLayer(TargetLayer);

        _logoDisplayer = new(Scene, AssetProvider, Services, TargetLayer);
        _logoDisplayer.LogoShowFinish += OnLogoShowFinishEvent;
        SubComponents.Add(_logoDisplayer);

        _loadingDisplayer = new(Scene, AssetProvider, Services, TargetLayer);
        SubComponents.Add(_loadingDisplayer);

        IntroSkipper Skipper = new(Scene, AssetProvider, Services);
        Skipper.SkippedIntro += OnIntroSkipEvent;
        SubComponents.Add(Skipper);
    }


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
        AnimationDone?.Invoke(this, EventArgs.Empty);
    }


    // Inherited methods.
    public override void OnLoad()
    {
        Services.FrameExecutor.SetFrame(_frame);
        base.OnLoad();
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);
    }
}