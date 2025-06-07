using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Scene.UI.Button;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuStartingUI : SceneComponentBase<MainMenuScene>
{
    // Fields
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            foreach (IBasicButton Button in _allButtons)
            {
                Button.IsEnabled = value;
            }
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            foreach (IBasicButton Button in _allButtons)
            {
                Button.IsVisible = value;
            }
        }
    }


    // Private static fields.
    private const string ASSET_NAME_LOGO = "main_logo";
    private const float LOGO_SIZE_Y = 0.15f;


    // Private fields.
    private readonly IBasicButton _playButton;
    private readonly IBasicButton _levelEditorButton;
    private readonly IBasicButton _optionsButton;
    private readonly IBasicButton _modsButton;
    private readonly IBasicButton _sourceButton;
    private readonly IBasicButton _exitButton;
    private readonly IBasicButton[] _allButtons;
    private readonly SpriteItem _logo;

    private bool _isEnabled = false;
    private bool _isVisible = false;

    private readonly ILayer _renderLayer;


    // Constructors.
    public MainMenuStartingUI(MainMenuScene scene, GenericServices services, ILayer renderLayer) : base(scene, services)
    {
        _renderLayer = renderLayer ?? throw new ArgumentNullException(nameof(renderLayer));

        IUIElementFactory ElementFactory = services.GetRequired<IUIElementFactory>();
        ISceneAssetProvider AssetProvider = services.GetRequired<ISceneAssetProvider>();

        _playButton = ElementFactory.CreateButton();
        _levelEditorButton = ElementFactory.CreateButton();
        _optionsButton = ElementFactory.CreateButton();
        _modsButton = ElementFactory.CreateButton();
        _sourceButton = ElementFactory.CreateButton();
        _exitButton = ElementFactory.CreateButton();

        _allButtons = new IBasicButton[] {
            _playButton,
            _levelEditorButton,
            _optionsButton,
            _modsButton,
            _sourceButton,
            _exitButton
        };

        _logo = new(AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_LOGO).CreateInstance());
    }


    // Private methods.
    private void InitLogo()
    {
        _logo.Size = new(_logo.FrameSize.X / _logo.FrameSize.Y * LOGO_SIZE_Y, LOGO_SIZE_Y);
        _logo.Position = new(0.5f, _logo.Size.Y / 2f);
        _renderLayer.AddItem(_logo);
    }

    private void InitButtons()
    {

    }



    // Inherited methods.
    public override void OnStart()
    {
        base.OnStart();

        InitLogo();
        InitButtons();
    }
}