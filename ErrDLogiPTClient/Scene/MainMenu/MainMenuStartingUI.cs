using ErrDLogiPTClient.Scene.InGame;
using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.UITextBox;
using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using GHEngine.GameFont;
using Microsoft.Xna.Framework;
using System;
using System.Text;

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
    private UIElementGroup _buttonGroup;

    private readonly MainMenuUIProperties _uiProperties;


    // Constructors.
    public MainMenuStartingUI(MainMenuScene scene, 
        GenericServices services,
        ILayer renderLayer,
        MainMenuUIProperties properties) : base(scene, services)
    {
        _uiProperties = properties ?? throw new ArgumentNullException(nameof(properties));
        _buttonGroup = new(renderLayer);
    }


    // Private methods.


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

        IBasicTextBox Box = ElementFactory.CreateTextBox();
        Box.Scale = 0.2f;
        Box.Dimensions = new(1f, 0.5f);
        Box.Position = new(0.75f, 0.5f);

        GHFontFamily FontFamily = SceneServices.GetRequired<ISceneAssetProvider>().GetAsset<GHFontFamily>(AssetType.Font, "main");

        StringBuilder Text = new();
        for (int i = 0; i < 15; i++)
        {
            if (i != 0)
            {
                Text.Append('\n');
            }
            Text.Append($"Hello World {i}");
        }

        Box.AddComponent(new(FontFamily, Text.ToString())
        {  FontSize = 0.025f, Mask = Color.Black });

        _buttonGroup.Add(_allButtons);
        _buttonGroup.Add(Box);
        _buttonGroup.Initialize();
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

        SpecificButtonInit();
    }

    protected void GenericInitSingleButton(IBasicButton button, Vector2 position)
    {
        button.Length = _uiProperties.ButtonLength;
        button.Scale = _uiProperties.ButtonScale;
        button.Position = position;
        button.IsDisabledOnClick = true;
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