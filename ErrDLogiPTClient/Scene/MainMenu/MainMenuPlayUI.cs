using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Service;
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


    public event EventHandler<EventArgs>? ClickExplore;
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
    private UIElementGroup _buttonGroup;

    private readonly MainMenuUIProperties _uiProperties;


    // Constructors.
    public MainMenuPlayUI(MainMenuScene scene, 
        GlobalServices services,
        ILayer layer,
        MainMenuUIProperties properties) : base(scene, services)
    {
        _uiProperties = properties ?? throw new ArgumentNullException(nameof(properties));
        _buttonGroup = new(layer);
    }



    // Private methods.
    private void InitButtons()
    {
        CreateButtons();
        _buttonGroup.AddElements(UIElementGroup.Z_INDEX_DEFAULT, _buttons!);
        _buttonGroup.Initialize();

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
        button.Position = position;
        button.IsDisabledOnClick = true;
        button.Length = _uiProperties.ButtonLength;
        button.Scale = _uiProperties.ButtonScale;
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
        button.MainClickAction = args =>
        {
            ClickExplore?.Invoke(this, EventArgs.Empty);
        };
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