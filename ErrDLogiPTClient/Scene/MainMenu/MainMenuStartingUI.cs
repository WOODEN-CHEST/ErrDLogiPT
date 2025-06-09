using ErrDLogiPTClient.Scene.InGame;
using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Scene.UI.Button;
using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuStartingUI : SceneComponentBase<MainMenuScene>, IMainMenuUISection
{
    // Fields
    public bool IsEnabled
    {
        get => _buttonGroup.IsEnabled;
        set => _buttonGroup.IsEnabled = value;
    }

    public bool IsVisible
    {
        get => _buttonGroup.IsVisible;
        set => _buttonGroup.IsVisible = value;
    }

    public event EventHandler<EventArgs> ClickPlay;


    // Private static fields.
    private const string ASSET_NAME_LOGO = "main_logo";

    private const string BUTTON_NAME_PLAY = "Play";
    private const string BUTTON_NAME_LEVEL_EDITOR = "Level Editor";
    private const string BUTTON_NAME_OPTIONS = "Options";
    private const string BUTTON_NAME_MODS = "Mods";
    private const string BUTTON_NAME_SOURCE = "Source";
    private const string BUTTON_NAME_EXIT = "Exit";



    // Private fields.
    private IBasicButton _playButton;
    private IBasicButton _levelEditorButton;
    private IBasicButton _optionsButton;
    private IBasicButton _modsButton;
    private IBasicButton _sourceButton;
    private IBasicButton _exitButton;
    private IBasicButton[]? _allButtons;
    private UIElementGroup _buttonGroup = new();

    private SpriteItem _logo;

    private readonly ILayer _renderLayer;
    private readonly MainMenuUIProperties _uiProperties;


    // Constructors.
    public MainMenuStartingUI(MainMenuScene scene, 
        GenericServices services,
        ILayer renderLayer,
        MainMenuUIProperties properties) : base(scene, services)
    {
        _renderLayer = renderLayer ?? throw new ArgumentNullException(nameof(renderLayer));
        _uiProperties = properties ?? throw new ArgumentNullException(nameof(properties));

    }


    // Private methods.
    private void InitLogo()
    {
        ISceneAssetProvider AssetProvider = SceneServices.GetRequired<ISceneAssetProvider>();
        _logo = new(AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_LOGO).CreateInstance());

        _logo.Origin = new(0.5f, 0.5f);
        _logo.Size = new Vector2(_logo.FrameSize.X / _logo.FrameSize.Y, 1f) * _uiProperties.LogoSizeY;
        _logo.Position = _uiProperties.LogoPosition;
        _renderLayer.AddItem(_logo);
    }

    protected void CreateButtons()
    {
        IUIElementFactory ElementFactory = SceneServices.GetRequired<IUIElementFactory>();

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
    }

    private void InitButtons()
    {
        CreateButtons();
        
        Vector2 Position = _uiProperties.ButtonStartingPosition;
        foreach (IBasicButton Button in _allButtons!)
        {
            GenericInitSingleButton(Button, Position);
            Position += new Vector2(0f, Button.ButtonBounds.Height * _uiProperties.ButtonOffsetY);
        }
        _buttonGroup.Add(_allButtons);

        SpecificButtonInit();
    }

    protected void GenericInitSingleButton(IBasicButton button, Vector2 position)
    {
        button.Initialize();
        button.Length = _uiProperties.ButtonLength;
        button.Scale = _uiProperties.ButtonScale;
        button.Position = position;
        button.IsDisabledOnClick = true;
        _renderLayer.AddItem(button);
    }

    private void SpecificButtonInit()
    {
        InitPlayButton(_playButton);
        InitLevelEditorButton(_levelEditorButton);
        InitOptionsButton(_optionsButton);
        InitModsButton(_modsButton);
        InitSourceButton(_sourceButton);
        InitExitButton(_exitButton);
    }

    private void InitPlayButton(IBasicButton button)
    {
        button.Text = BUTTON_NAME_PLAY;
        button.MainClickAction = args =>
        {
            ClickPlay?.Invoke(this, EventArgs.Empty);
        };
    }

    private void InitModsButton(IBasicButton button)
    {
        button.Text = BUTTON_NAME_MODS;
    }

    private void InitSourceButton(IBasicButton button)
    {
        button.Text = BUTTON_NAME_SOURCE;
    }

    private void InitLevelEditorButton(IBasicButton button)
    {
        button.Text = BUTTON_NAME_LEVEL_EDITOR;
    }

    private void InitOptionsButton(IBasicButton button)
    {
        button.Text = BUTTON_NAME_OPTIONS;
    }

    private void InitExitButton(IBasicButton button)
    {
        button.Text = BUTTON_NAME_EXIT;
        button.MainClickAction = args => SceneServices.GetRequired<ISceneExecutor>().Exit();
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        InitLogo();
        InitButtons();
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPreComponent();
    }

    protected override void HandleUpdatePostComponent(IProgramTime time)
    {
        base.HandleUpdatePostComponent(time);

        _buttonGroup.Update(time);
    }
}