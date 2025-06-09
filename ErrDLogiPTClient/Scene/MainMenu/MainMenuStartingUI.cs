using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Scene.UI.Button;
using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            foreach (IBasicButton Button in _allButtons ?? Array.Empty<IBasicButton>())
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
            foreach (IBasicButton Button in _allButtons ?? Array.Empty<IBasicButton>())
            {
                Button.IsVisible = value;
            }
        }
    }


    // Private static fields.
    private const string ASSET_NAME_LOGO = "main_logo";
    private const float LOGO_SIZE_Y = 0.15f;

    private const string BUTTON_NAME_PLAY = "Play";
    private const string BUTTON_NAME_LEVEL_EDITOR = "Level Editor";
    private const string BUTTON_NAME_OPTIONS = "Options";
    private const string BUTTON_NAME_MODS = "Mods";
    private const string BUTTON_NAME_SOURCE = "Source";
    private const string BUTTON_NAME_EXIT = "Exit";

    private const float BUTTON_LENGTH = 12f;
    private const float BUTTON_SCALE = 0.1f;
    private const float LOGO_OFFSET_Y = 0.05f;
    private const float BUTTON_BOUNDS_OFFSET_SCALE = 1.15f;


    // Private fields.
    private IBasicButton _playButton;
    private IBasicButton _levelEditorButton;
    private IBasicButton _optionsButton;
    private IBasicButton _modsButton;
    private IBasicButton _sourceButton;
    private IBasicButton _exitButton;
    private IBasicButton[]? _allButtons;
    private SpriteItem _logo;

    private bool _isEnabled = false;
    private bool _isVisible = false;

    private readonly ILayer _renderLayer;


    // Constructors.
    public MainMenuStartingUI(MainMenuScene scene, GenericServices services, ILayer renderLayer) : base(scene, services)
    {
        _renderLayer = renderLayer ?? throw new ArgumentNullException(nameof(renderLayer));
    }


    // Private methods.
    private void InitLogo()
    {
        ISceneAssetProvider AssetProvider = SceneServices.GetRequired<ISceneAssetProvider>();
        _logo = new(AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_LOGO).CreateInstance());

        _logo.Origin = new(0.5f, 0.5f);
        _logo.Size = new(_logo.FrameSize.X / _logo.FrameSize.Y * LOGO_SIZE_Y, LOGO_SIZE_Y);
        _logo.Position = new(0.5f, (_logo.Size.Y / 2f) + LOGO_OFFSET_Y);
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

        Vector2 Position = _logo.Position + new Vector2(0f, _logo.Size.Y + LOGO_OFFSET_Y);
        foreach (IBasicButton Button in _allButtons!)
        {
            GenericInitSingleButton(Button, Position);
            Position += new Vector2(0f, Button.ButtonBounds.Height * BUTTON_BOUNDS_OFFSET_SCALE);
        }

        SpecificButtonInit();
    }

    protected void GenericInitSingleButton(IBasicButton button, Vector2 position)
    {
        button.Initialize();
        button.IsVisible = IsVisible;
        button.IsEnabled = IsEnabled;
        button.Length = BUTTON_LENGTH;
        button.Scale = BUTTON_SCALE;
        button.Position = position;
        button.IsDisabledOnClick = true;
        _renderLayer.AddItem(button);
    }

    private void SpecificButtonInit()
    {
        _playButton.Text = BUTTON_NAME_PLAY;
        _levelEditorButton.Text = BUTTON_NAME_LEVEL_EDITOR;
        _optionsButton.Text = BUTTON_NAME_OPTIONS;
        _modsButton.Text = BUTTON_NAME_MODS;
        _sourceButton.Text = BUTTON_NAME_SOURCE;
        _exitButton.Text = BUTTON_NAME_EXIT;

        InitExitButton(_exitButton);
    }

    private void InitExitButton(IBasicButton button)
    {
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

        foreach (IBasicButton Button in _allButtons!)
        {
            Button.Update(time);
        }
    }
}