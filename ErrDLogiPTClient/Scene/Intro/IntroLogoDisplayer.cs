using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

public class IntroLogoDisplayer : SceneComponentBase<IntroScene>
{
    // Fields.
    public event EventHandler<EventArgs>? LogoShowFinish;


    // Private static fields.
    private const string ASSET_NAME_LOGO = "main_logo";
    private static readonly TimeSpan FADE_IN_DURATION = TimeSpan.FromSeconds(2d);
    private static readonly TimeSpan FADE_STAY_DURATION = TimeSpan.FromSeconds(3d);
    private static readonly TimeSpan FADE_OUT_DURATION = TimeSpan.FromSeconds(2d);
    private static readonly TimeSpan FULL_FADE_DURATION = FADE_IN_DURATION + FADE_STAY_DURATION + FADE_OUT_DURATION;
    private const float SCALE_START = 0.6f;
    private const float SCALE_END = 0.8f;


    // Private fields.
    private SpriteItem _logo;
    private TimeSpan _fadeTime;

    private readonly ILayer _logoLayer;

    private Vector2 _logoSizeMax;
    private Vector2 _logoSizeMin;
    private bool _isFadeFinished = false;


    // Constructors.
    public IntroLogoDisplayer(IntroScene scene,
        ISceneAssetProvider assetProvider,
        IGameServices services,
        ILayer logoLayer) : base(scene, assetProvider, services)
    {
        _logoLayer = logoLayer ?? throw new ArgumentNullException(nameof(logoLayer));
    }


    // Methods.
    public void SkipAnimation()
    {
        _fadeTime = FADE_IN_DURATION + FADE_STAY_DURATION + FADE_OUT_DURATION;
        _logo.Opacity = 0f;
        _isFadeFinished = true;
    }


    // Private methods.
    private void UpdateLogoAnimation(IProgramTime time)
    {
        _fadeTime += time.PassedTime;

        if (_fadeTime > FULL_FADE_DURATION)
        {
            _isFadeFinished = true;
            _logo.Opacity = 0f;
            LogoShowFinish?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (_fadeTime < FADE_IN_DURATION)
        {
            _logo.Opacity = (float)(_fadeTime.TotalSeconds / FADE_IN_DURATION.TotalSeconds);
        }
        else if (_fadeTime < FADE_IN_DURATION + FADE_STAY_DURATION)
        {
            _logo.Opacity = 1f;
        }
        else if (_fadeTime < FULL_FADE_DURATION)
        {
            _logo.Opacity = 1f - (float)((_fadeTime - (FADE_IN_DURATION + FADE_STAY_DURATION)) / FADE_OUT_DURATION);
        }

        float SizeLerpAmount = (float)(_fadeTime.TotalSeconds / (FULL_FADE_DURATION).TotalSeconds);
        _logo.Size = _logoSizeMin + ((_logoSizeMax - _logoSizeMin) * SizeLerpAmount);
    }


    // Inherited methods.
    public override void OnLoad()
    {
        base.OnLoad();

        ISpriteAnimation Animation = AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_LOGO)!;

        _logo = new(Animation.CreateInstance());

        Vector2 LogoBaseSize = new Vector2(1f, _logo.FrameSize.Y / _logo.FrameSize.X);
        _logoSizeMin = LogoBaseSize * SCALE_START;
        _logoSizeMax = LogoBaseSize * SCALE_END;

        _logo.Origin = new(0.5f, 0.5f);
        _logo.Position = new(0.5f, 0.5f);
        _logo.IsSizeAdjusted = true;
        _logo.Opacity = 0f;

        _logoLayer.AddItem(_logo);
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);

        if (!_isFadeFinished)
        {
            UpdateLogoAnimation(time);
        }
    }
}