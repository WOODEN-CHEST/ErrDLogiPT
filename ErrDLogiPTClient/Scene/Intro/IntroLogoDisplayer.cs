using ErrDLogiPTClient.Service;
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

public class IntroLogoDisplayer : SceneComponentBase
{
    // Fields.
    public event EventHandler<EventArgs>? LogoShowFinish;


    // Protected fields.
    protected virtual TimeSpan FadeInDuration
    {
        get => _fadeInDuration;
        set
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentException("Fade in duration cannot be negative");
            }
            _fadeInDuration = value;
        }
    }

    protected virtual TimeSpan FadeStayDuration
    {
        get => _fadeStayDuration;
        set
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentException("Fade stay duration cannot be negative");
            }
            _fadeStayDuration = value;
        }
    }

    protected virtual TimeSpan FadeOutDuration
    {
        get => _fadeOutDuration;
        set
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentException("Fade out duration cannot be negative");
            }
            _fadeOutDuration = value;
        }
    }

    protected virtual TimeSpan FulLFadeDuration => FadeInDuration + FadeStayDuration + FadeOutDuration;

    protected virtual float ScaleStart
    {
        get => _scaleStart;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid start scale: {value}", nameof(value));
            }
            _scaleStart = value;
        }
    }

    protected virtual float ScaleEnd
    {
        get => _scaleEnd;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid end scale: {value}", nameof(value));
            }
            _scaleEnd = value;
        }
    }

    protected virtual SpriteItem Logo
    {
        get => _logo;
        set => _logo = value ?? throw new ArgumentNullException(nameof(value));
    }

    protected virtual ILayer LogoLayer => _logoLayer;


    // Private static fields.
    private const string ASSET_NAME_LOGO = "main_logo";


    // Private fields.
    private readonly ILayer _logoLayer;

    private TimeSpan _fadeInDuration = TimeSpan.FromSeconds(2d);
    private TimeSpan _fadeStayDuration = TimeSpan.FromSeconds(3d);
    private TimeSpan _fadeOutDuration = TimeSpan.FromSeconds(2d);
    private float _scaleStart = 0.6f;
    private float _scaleEnd = 0.8f;

    private SpriteItem _logo;
    private TimeSpan _fadeTime;
    private Vector2 _logoSizeEnd;
    private Vector2 _logoSizeStart;
    private bool _isFadeFinished = false;


    // Constructors.
    public IntroLogoDisplayer(IGameScene scene, IGenericServices sceneServices, ILayer logoLayer)
        : base(scene, sceneServices)
    {
        _logoLayer = logoLayer ?? throw new ArgumentNullException(nameof(logoLayer));
    }


    // Methods.
    public void SkipAnimation()
    {
        _fadeTime = FulLFadeDuration;
        Logo.Opacity = 0f;
        _isFadeFinished = true;
    }


    // Protected methods.
    protected void UpdateLogoAnimation(IProgramTime time)
    {
        _fadeTime += time.PassedTime;

        if (_fadeTime >= FulLFadeDuration)
        {
            _isFadeFinished = true;
            Logo.Opacity = 0f;
            LogoShowFinish?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (_fadeTime < FadeInDuration)
        {
            Logo.Opacity = (FadeInDuration.Ticks <= 0)
                ? 1f
                : ((float)(_fadeTime.TotalSeconds / FadeInDuration.TotalSeconds));
        }
        else if (_fadeTime < FadeInDuration + FadeStayDuration)
        {
            Logo.Opacity = 1f;
        }
        else if (_fadeTime < FulLFadeDuration)
        {
            Logo.Opacity = (FadeOutDuration.Ticks <= 0)
                ? 0f
                : (1f - (float)((_fadeTime - (FadeInDuration + FadeStayDuration)) / FadeOutDuration));
        }

        float SizeLerpAmount = (FulLFadeDuration.Ticks <= 0)
            ? 1f
            : ((float)(_fadeTime.TotalSeconds / (FulLFadeDuration).TotalSeconds));

        Logo.Size = _logoSizeStart + ((_logoSizeEnd - _logoSizeStart) * SizeLerpAmount);
    }

    protected virtual void UpdateOnNonFinishedFade(IProgramTime time) { }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        ISceneAssetProvider AssetProvider = SceneServices.GetRequired<ISceneAssetProvider>();
        ISpriteAnimation Animation = AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_LOGO)!;

        Logo = new(Animation.CreateInstance());

        Vector2 LogoBaseSize = new Vector2(1f, _logo.FrameSize.Y / _logo.FrameSize.X);
        _logoSizeStart = LogoBaseSize * ScaleStart;
        _logoSizeEnd = LogoBaseSize * ScaleEnd;

        Logo.Origin = new(0.5f, 0.5f);
        Logo.Position = new(0.5f, 0.5f);
        Logo.IsSizeAdjusted = true;
        Logo.Opacity = 0f;

        LogoLayer.AddItem(_logo);
        AssetProvider.RegisterRenderedItem(_logo);
    }

    protected override void HandleUpdatePreComponent(IProgramTime time)
    {
        base.HandleUpdatePreComponent(time);

        if (!_isFadeFinished)
        {
            UpdateLogoAnimation(time);
            UpdateOnNonFinishedFade(time);
        }
    }
}