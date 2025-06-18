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

public class MainMenuLogoDisplayer : SceneComponentBase, IMainMenuUISection
{
    // Fields.
    public float LogoSizeY { get; } = 0.15f;
    public float LogoOffsetY { get; } = 0.05f;
    public Vector2 LogoPosition { get; private init; }

    public bool IsEnabled
    {
        get => true;
        set { }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible == value)
            {
                return;
            }

            _isVisible = value;

            if (_logo != null)
            {
                UpdateLogoVisibility();
            }
        }
    }


    // Private static fields.
    private const string ASSET_NAME_LOGO = "main_logo";


    // Private fields.
    protected readonly Vector2 _logoPosition;
    protected readonly float _logoSizeY;
    private readonly ILayer _renderLayer;
    protected SpriteItem? _logo;
    private bool _isVisible = false;


    // Constructors.
    public MainMenuLogoDisplayer(IGameScene scene,
        IGenericServices services,
        ILayer renderLayer)
        : base(scene, services)
    {
        _renderLayer = renderLayer ?? throw new ArgumentNullException(nameof(renderLayer));
        LogoPosition = new(0.5f, LogoSizeY);
    }


    // Private methods.
    private void InitLogo()
    {
        ISceneAssetProvider AssetProvider = SceneServices.GetRequired<ISceneAssetProvider>();
        _logo = new(AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_LOGO).CreateInstance());

        _logo.Origin = new(0.5f, 0.5f);
        _logo.Size = new Vector2(_logo.FrameSize.X / _logo.FrameSize.Y, 1f) * LogoSizeY;
        _logo.Position = LogoPosition;
        _renderLayer.AddItem(_logo);
    }

    private void UpdateLogoVisibility()
    {
        _logo!.IsVisible = IsVisible;

        _renderLayer.RemoveItem(_logo);

        if (IsVisible)
        {
            _renderLayer.AddItem(_logo);
        }
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        InitLogo();
        UpdateLogoVisibility();

        base.HandleLoadPreComponent();
    }
}