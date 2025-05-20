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
    // Private static fields.
    private const string ASSET_NAME_LOGO = "main_logo";
    private static readonly TimeSpan FADE_IN_DURATION = TimeSpan.FromSeconds(1d);
    private static readonly TimeSpan FADE_STAY_DURATION = TimeSpan.FromSeconds(3d);
    private static readonly TimeSpan FADE_OUT_DURATION = TimeSpan.FromSeconds(1d);
    private const float SCALE_START = 0.6f;
    private const float SIZE_END = 0.8f;


    // Private fields.
    private SpriteItem _logo;
    private TimeSpan _fadeTime;
    private readonly ILayer _logoLayer;


    // Constructors.
    public IntroLogoDisplayer(IntroScene scene,
        ISceneAssetProvider assetProvider,
        IGameServices services,
        ILayer logoLayer) : base(scene, assetProvider, services)
    {
        _logoLayer = logoLayer ?? throw new ArgumentNullException(nameof(logoLayer));
    }


    // Private methods.
    private void UpdateLogoAnimation(IProgramTime time)
    {
        _logo.Position += new Vector2((float)(time.PassedTime.TotalSeconds * 10d), 0f);
        _fadeTime += time.PassedTime;

        if (_fadeTime < FADE_IN_DURATION)
        {
            _logo.Opacity = (float)(_fadeTime.TotalSeconds / FADE_IN_DURATION.TotalSeconds);
        }
        else if (_fadeTime < FADE_IN_DURATION + FADE_STAY_DURATION)
        {
            _logo.Opacity = 1f;
        }
        else
        {
            _logo.Opacity = 1f - (float)((_fadeTime - (FADE_IN_DURATION + FADE_STAY_DURATION)) / FADE_OUT_DURATION);
        }
    }


    // Inherited methods.
    public override void OnLoad()
    {
        base.OnLoad();

        ISpriteAnimation Animation = AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_LOGO)!;

        _logo = new(Animation.CreateInstance());
        _logo.Size = new Vector2(1f, _logo.FrameSize.Y / _logo.FrameSize.X) * SCALE_START;
        _logo.Origin = new(0.5f, 0.5f);
        _logo.Position = new(0.5f, 0.5f);
        _logo.IsSizeAdjusted = true;

        _logoLayer.AddItem(_logo);
    }

    public override void OnStart()
    {
        base.OnStart();
        _logo.Opacity = 0f;
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);
        UpdateLogoAnimation(time);
    }
}