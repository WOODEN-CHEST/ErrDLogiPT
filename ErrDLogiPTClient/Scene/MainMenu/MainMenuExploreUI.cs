using ErrDLogiPTClient.OS;
using ErrDLogiPTClient.Registry;
using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.Dropdown;
using ErrDLogiPTClient.Scene.UI.UITextBox;
using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Item;
using GHEngine.GameFont;
using GHEngine.Screen;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuExploreUI : SceneComponentBase<MainMenuScene>, IMainMenuUISection
{
    // Fields.
    public bool IsEnabled
    {
        get => _elementGroup.IsVisible;
        set => _elementGroup.IsVisible = value;
    }

    public bool IsVisible
    {
        get => _elementGroup.IsEnabled;
        set => _elementGroup.IsEnabled = value;
    }

    public event EventHandler<EventArgs>? ClickBack;
    public event EventHandler<EventArgs>? ClickBootIntoOS;


    // Private static fields.
    private const float MAIN_BUTTON_OFFSET_Y = 0.075f;
    private const float MAIN_BUTTON_OFFSET_X = MAIN_BUTTON_OFFSET_Y / 2f;
    private const string BUTTON_TEXT_BACK = "Back";
    private const string BUTTON_TEXT_BOOT_INTO_OS = "Boot into OS";
    private const float DROPDOWN_LIST_RELATIVE_SCALE = 0.5f;
    private const float DROPDOWN_LIST_RELATIVE_LENGTH = 1f;
    private const float TEXTBOX_OFFSET_Y = 0.05f;


    // Private fields.
    private readonly MainMenuUIProperties _uiProperties;

    private IBasicDropdownList<IGameOSDefinition> _osSelectionDropdown;
    private IBasicButton _backButton;
    private IBasicButton _bootIntoOSButton;
    private IBasicTextBox _actionTextbox;
    private readonly UIElementGroup _elementGroup;


    // Constructors.
    public MainMenuExploreUI(MainMenuScene scene,
        GenericServices services,
        ILayer renderLayer,
        MainMenuUIProperties properties)
        : base(scene, services)
    {
        _uiProperties = properties ?? throw new ArgumentNullException(nameof(properties));
        _elementGroup = new(renderLayer);
    }


    // Private methods.
    private IEnumerable<IGameOSDefinition> GetOSDefinitions()
    {
        return SceneServices.GetRequired<IGameRegistryStorage>().GetRegistry<IGameOSDefinition>()
            ?? Enumerable.Empty<IGameOSDefinition>();
    }

    private void UpdatePositions(IntVector windowSize)
    {
        float AspectRatio = (float)windowSize.X / (float)windowSize.Y;
        Vector2 ButtonDimensions = new(_backButton.ButtonBounds.Width, _backButton.ButtonBounds.Height);

        Vector2 BothButtonOffset =
            GHMath.GetWindowAdjustedVector(new(0f, -MAIN_BUTTON_OFFSET_Y), AspectRatio)
            - new Vector2(0f, ButtonDimensions.Y / 2f);

        _backButton.Position = new Vector2(0.5f, 1f)
            + BothButtonOffset
            + new Vector2 (MAIN_BUTTON_OFFSET_X + ButtonDimensions.X / 2f, 0f);

        _bootIntoOSButton.Position = new Vector2(0.5f, 1f)
            + BothButtonOffset
            - new Vector2(MAIN_BUTTON_OFFSET_X + ButtonDimensions.X / 2f, 0f);

        _actionTextbox.Position = new(0.5f, _actionTextbox.Bounds.Height / 2f + TEXTBOX_OFFSET_Y);
        _osSelectionDropdown.Position = _actionTextbox.Position
            + new Vector2(0f, _actionTextbox.Bounds.Height + TEXTBOX_OFFSET_Y);
    }

    private void CreateElements()
    {
        IUIElementFactory Factory = SceneServices.GetRequired<IUIElementFactory>();

        _backButton = Factory.CreateButton();
        _bootIntoOSButton = Factory.CreateButton();
        _osSelectionDropdown = Factory.CreateDropdownList<IGameOSDefinition>();
        _actionTextbox = Factory.CreateTextBox();
    }

    private void InitElements()
    {
        CreateElements();

        InitActionText(_actionTextbox);
        InitOSDropdownList(_osSelectionDropdown);

        GenericButtonInit(_backButton);
        GenericButtonInit(_bootIntoOSButton);
        InitBackButton(_backButton);
        InitBootIntoOSButton(_bootIntoOSButton);

        _elementGroup.AddElements(UIElementGroup.Z_INDEX_DEFAULT,
            _backButton,
            _bootIntoOSButton,
            _osSelectionDropdown,
            _actionTextbox);

        _elementGroup.Initialize();

        UpdateBootIntoOSButton();

        UpdatePositions(SceneServices.GetRequired<IDisplay>().CurrentWindowSize);
    }

    private void GenericButtonInit(IBasicButton button)
    {
        button.Length = _uiProperties.ButtonLength;
        button.Scale = _uiProperties.ButtonScale;
        button.IsDisabledOnClick = true;
    }

    private void InitOSDropdownList(IBasicDropdownList<IGameOSDefinition> dropdownList)
    {
        IEnumerable<IGameOSDefinition> OperatingSystems = GetOSDefinitions();
        dropdownList.SetElements(OperatingSystems.Select(
            sys => new DropdownListElement<IGameOSDefinition>(sys.Name, sys)));

        dropdownList.SelectionUpdate += OnSelectOSEvent;
        dropdownList.Scale = _uiProperties.ButtonScale * DROPDOWN_LIST_RELATIVE_SCALE;
        dropdownList.Length = _uiProperties.ButtonLength * DROPDOWN_LIST_RELATIVE_LENGTH;
    }

    private void InitActionText(IBasicTextBox text)
    {
        ISceneAssetProvider AssetProvider = SceneServices.GetRequired<ISceneAssetProvider>();
        GHFontFamily Font = AssetProvider.GetAsset<GHFontFamily>(AssetType.Font, _uiProperties.DefaultFontAssetName);

        text.AddComponent(new(Font,
            "Explore an operating system without any active viruses. Simply select the operating " +
            $"system you wish to explore and click \"{BUTTON_TEXT_BOOT_INTO_OS}\"!")
        {
            FontSize = _uiProperties.DefaultFontSize,
        });

        text.Alignment = TextAlignOption.Center;
        text.Scale = _uiProperties.TextBoxScale;
        text.Dimensions = new(2f, 0.4f);
    }

    private void InitBackButton(IBasicButton button)
    {
        button.Text = BUTTON_TEXT_BACK;
        button.MainClickAction = args =>
        {
            ClickBack?.Invoke(this, EventArgs.Empty);
        };
    }

    private void InitBootIntoOSButton(IBasicButton button)
    {
        button.Text = BUTTON_TEXT_BOOT_INTO_OS;
        button.MainClickAction = args =>
        {
            ClickBootIntoOS?.Invoke(this, EventArgs.Empty);
        };
    }

    private void UpdateBootIntoOSButton()
    {
        IUIElementFactory Factory = SceneServices.GetRequired<IUIElementFactory>();

        if (_osSelectionDropdown.SelectedElementCount <= 0)
        {
            _bootIntoOSButton.ButtonColor = Factory.UnavailableColor;
            _elementGroup.SetAbilityOverride(_bootIntoOSButton, false);
        }
        else
        {
            _bootIntoOSButton.ButtonColor = Factory.NormalColor;
            _elementGroup.SetAbilityOverride(_bootIntoOSButton, null);
        }
    }

    private void OnSelectOSEvent(object? sender, BasicDropdownSelectionUpdateEventArgs<IGameOSDefinition> args)
    {
        args.AddSuccessAction(UpdateBootIntoOSButton);
    }

    protected void OnScreenSizeChangeEvent(object? sender, ScreenSizeChangeEventArgs args)
    {
        UpdatePositions(args.NewSize);
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        InitElements();
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPostComponent();

        SceneServices.GetRequired<IDisplay>().ScreenSizeChange += OnScreenSizeChangeEvent;
    }

    protected override void HandleEndPreComponent()
    {
        base.HandleEndPreComponent();

        SceneServices.GetRequired<IDisplay>().ScreenSizeChange -= OnScreenSizeChangeEvent;
    }

    protected override void HandleUpdatePreComponent(IProgramTime time)
    {
        base.HandleUpdatePreComponent(time);

        _elementGroup.Update(time);
    }
}