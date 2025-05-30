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

public class IntroRenderExecutor : SceneComponentBase<IntroScene>
{
    // Fields.
    public bool IsLoadingShown { get; set; } = true;
    public bool IsAnimationDone { get; private set; } = false;
    public event EventHandler<EventArgs>? AnimationDone;



    // Private static fields.
    private const string LOGO_LAYER_NAME = "main";


    // Private fields.
    private readonly IFrameExecutor _frameExecutor;

    private readonly IntroLogoDisplayer _logoDisplayer;
    private readonly IntroLoadingDisplayer _loadingDisplayer;
    private readonly IGameFrame _frame;


    // Constructors.
    public IntroRenderExecutor(IntroScene scene,
        ISceneAssetProvider assetProvider,
        IUserInput userInput,
        IFrameExecutor frameExecutor)
        : base(scene)
    {
        _frameExecutor = frameExecutor ?? throw new ArgumentNullException(nameof(frameExecutor));

        _frame = new GHGameFrame();

        ILayer TargetLayer = new GHLayer(LOGO_LAYER_NAME);
        _frame.AddLayer(TargetLayer);

        _logoDisplayer = new(TypedScene, TargetLayer, assetProvider);
        _logoDisplayer.LogoShowFinish += OnLogoShowFinishEvent;
        SubComponents.Add(_logoDisplayer);

        _loadingDisplayer = new(TypedScene, assetProvider, TargetLayer);
        SubComponents.Add(_loadingDisplayer);

        IntroSkipper Skipper = new(TypedScene, userInput);
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
        base.OnLoad();
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);
        _frameExecutor.SetFrame(_frame);
    }
}