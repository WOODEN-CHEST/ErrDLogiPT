using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Scene.UI.Button;
using GHEngine;
using GHEngine.Frame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuPlayUI : SceneComponentBase<MainMenuScene>, IMainMenuUISection
{
    // Fields.
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

    public event EventHandler<EventArgs>? ClickBack;


    // Private static fields.
    private const string BUTTON_NAME_LEVELS = "Levels";
    private const string BUTTON_NAME_ONLINE_LEVELS = "Online Levels";
    private const string BUTTON_NAME_EXPLORE = "Explore";
    private const string BUTTON_NAME_BACK = "Back";


    // Private fields.
    private IBasicButton _levelsButton;
    private IBasicButton _onlineLevelsButton;
    private IBasicButton _exploreButton;
    private IBasicButton _backButton;
    private IBasicButton[]? _buttons;
    private UIElementGroup _buttonGroup = new();

    private readonly ILayer _layer;
    private readonly MainMenuUIProperties _uiProperties;


    // Constructors.
    public MainMenuPlayUI(MainMenuScene scene, 
        GenericServices services,
        ILayer layer,
        MainMenuUIProperties properties) : base(scene, services)
    {
        _layer = layer ?? throw new ArgumentNullException(nameof(layer));
        _uiProperties = properties ?? throw new ArgumentNullException(nameof(properties));
    }



    // Private methods.
    private void InitButtons()
    {
        CreateButtons();
        _buttonGroup.Add(_buttons!);

        Vector2 Position = _uiProperties.ButtonStartingPosition;
        foreach (IBasicButton Button in _buttons!)
        {
            GenericButtonInit(Button, Position);
            Position += new Vector2(0f, Button.ButtonBounds.Height * _uiProperties.ButtonOffsetY);
        }

        InitSpecificButtons();
    }

    private void CreateButtons()
    {
        IUIElementFactory Factory = SceneServices.GetRequired<IUIElementFactory>();
        _levelsButton = Factory.CreateButton();
        _onlineLevelsButton = Factory.CreateButton();
        _exploreButton = Factory.CreateButton();
        _backButton = Factory.CreateButton();

        _buttons = new IBasicButton[]
        {
            _levelsButton,
            _onlineLevelsButton,
            _exploreButton,
            _backButton
        };
    }

    private void GenericButtonInit(IBasicButton button, Vector2 position)
    {
        button.Initialize();
        button.Position = position;
        button.IsDisabledOnClick = true;
        button.Length = _uiProperties.ButtonLength;
        button.Scale = _uiProperties.ButtonScale;
        _layer.AddItem(button);
    }

    private void InitSpecificButtons()
    {
        InitLevelsButton(_levelsButton);
        InitOnlineLevelsButton(_onlineLevelsButton);
        InitExploreButton(_exploreButton);
        InitBackButton(_backButton);
    }

    private void InitLevelsButton(IBasicButton button)
    {
        button.Text = BUTTON_NAME_LEVELS;
    }
    private void InitOnlineLevelsButton(IBasicButton button)
    {
        button.Text = BUTTON_NAME_ONLINE_LEVELS;
    }

    private void InitExploreButton(IBasicButton button)
    {
        button.Text = BUTTON_NAME_EXPLORE;
    }

    private void InitBackButton(IBasicButton button)
    {
        button.Text = BUTTON_NAME_BACK;
        button.MainClickAction = args =>
        {
            ClickBack?.Invoke(this, EventArgs.Empty);
        };
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        InitButtons();
    }

    protected override void HandleUpdatePreComponent(IProgramTime time)
    {
        base.HandleUpdatePreComponent(time);

        _buttonGroup.Update(time);
    }
}