using ErrDLogiPTClient.Service;
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

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuBackground : SceneComponentBase<MainMenuScene>
{
    // Private static fields.
    private const string ASSET_NAME_PIXEL = "pixel";
    private static readonly Color BACKGROUND_COLOR = new(0.125f, 0.125f, 0.125f, 1f);


    // Private fields.
    private readonly ILayer _bgLayer;
    private SpriteItem _background;


    // Constructors.
    public MainMenuBackground(MainMenuScene scene, GlobalServices services, ILayer bgLayer) : base(scene, services)
    {
        _bgLayer = bgLayer ?? throw new ArgumentNullException(nameof(bgLayer));
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        _background = new(SceneServices
            .GetRequired<ISceneAssetProvider>()
            .GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_PIXEL)
            .CreateInstance())
        {
            Mask = BACKGROUND_COLOR,
            IsPositionAdjusted = false,
            IsSizeAdjusted = false,
            Position = new(0f, 0f),
            Size = new(1f, 1f)
        };

        _bgLayer.AddItem(_background, float.NegativeInfinity);

        base.HandleLoadPreComponent();
    }
}