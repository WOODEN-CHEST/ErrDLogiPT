using ErrDLogiPTClient.Service;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuBackground : SceneComponentBase
{
    // Protected fields.
    protected virtual ILayer TargetLayer
    {
        get => _targetLayer;
        set => _targetLayer = value ?? throw new ArgumentNullException(nameof(TargetLayer));
    }
    protected virtual Color BackgroundColor { get; set; } = new(0.125f, 0.125f, 0.125f, 1f);
    protected virtual string? BackgroundAssetName { get; set; } = "pixel";
    protected virtual SpriteItem? BackgroundSprite { get; set; } = null;


    // Private fields.

    private ILayer _targetLayer;
    private SpriteItem _background;


    // Constructors.
    public MainMenuBackground(IGameScene scene, IGenericServices services,  ILayer bgLayer)
        : base(scene, services)
    {
        TargetLayer = bgLayer;
    }


    // Protected methods.
    protected virtual SpriteItem CreateBackgroundSprite(ISceneAssetProvider assetProvider)
    {
        ISpriteAnimation Animation = assetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, BackgroundAssetName);

        return new(Animation.CreateInstance())
        {
            Mask = BackgroundColor,
            IsPositionAdjusted = false,
            IsSizeAdjusted = false,
            Position = new(0f, 0f),
            Size = new(1f, 1f)
        };
    }

    protected virtual void PostProcessBackgroundSprite(SpriteItem sprite) { }

    protected virtual void AddBackgroundSpriteToLayer()
    {
        _targetLayer.AddItem(_background, float.NegativeInfinity);
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();
        _background = CreateBackgroundSprite(SceneServices.GetRequired<ISceneAssetProvider>());
        PostProcessBackgroundSprite(_background);
        AddBackgroundSpriteToLayer();
    }
}