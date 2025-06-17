using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Service;
using GHEngine.Frame;


namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuUIExecutor : SceneComponentBase<MainMenuScene>
{
    // Fields.



    // Private static fields.
    private const string LAYER_NAME_BACKGROUND = "background";
    private const string LAYER_NAME_FOREGROUND = "foreground";

    private const float BUTTON_LENGTH = 12f;
    private const float BUTTON_SCALE = 0.1f;
    private const float BUTTON_BOUNDS_OFFSET_SCALE = 1.15f;
    private const string ASSET_NAME_FONT = "main";
    private const float DEFAULT_FONT_SIZE = 0.03f;
    private const float DEFAULT_TEXTBOX_SCALE = 0.3f;


    // Private fields.
    private IGameFrame _frame;
    private ILayer _backgroundLayer;
    private ILayer _foregroundLayer;

    private MainMenuBackground _background;
    private MainMenuLogoDisplayer _logo;

    private MainMenuStartingUI _startingUI;
    private MainMenuPlayUI _playUI;
    private MainMenuExploreUI _exploreUI;


    // Constructors.
    public MainMenuUIExecutor(MainMenuScene scene, GlobalServices sceneServices) : base(scene, sceneServices) { }


    // Private methods.
    private void InitEventHandlers()
    {
        InitStartingEventHandlers();
        InitPlayEventHandlers();
        InitExploreEventHandlers();
    }

    private void SwitchUISections(IMainMenuUISection oldSection, IMainMenuUISection newSection)
    {
        oldSection.IsEnabled = false;
        oldSection.IsVisible = false;

        newSection.IsEnabled = true;
        newSection.IsVisible = true;
    }

    private void InitStartingEventHandlers()
    {
        _startingUI.ClickPlay += (sender, args) => SwitchUISections(_startingUI, _playUI);
    }

    private void InitPlayEventHandlers()
    {
        _playUI.ClickBack += (sender, args) => SwitchUISections(_playUI, _startingUI);
        _playUI.ClickExplore += (sender, args) =>
        {
            SwitchUISections(_playUI, _exploreUI);
            _logo.IsVisible = false;
        };
    }

    private void InitExploreEventHandlers()
    {
        _exploreUI.ClickBack += (sender, args) =>
        {
            SwitchUISections(_exploreUI, _playUI);
            _logo.IsVisible = true;
        };
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        _frame = new GHGameFrame();

        _backgroundLayer = new GHLayer(LAYER_NAME_BACKGROUND);
        _foregroundLayer = new GHLayer(LAYER_NAME_FOREGROUND);
        _frame.AddLayer(_backgroundLayer);
        _frame.AddLayer(_foregroundLayer);

        IUIElementFactory Factory = SceneServices.GetRequired<IUIElementFactory>();
        Factory.LoadAssets();

        _logo = new(TypedScene, SceneServices, _foregroundLayer);
        _logo.IsVisible = true;
        _logo.IsEnabled = true;
        AddComponent(_logo);

        float LogoOffsetY = _logo.LogoOffsetY;
        float LogoSizeY = _logo.LogoSizeY;
        MainMenuUIProperties UIProperties = new()
        {
            ButtonOffsetY = BUTTON_BOUNDS_OFFSET_SCALE,
            ButtonLength = BUTTON_LENGTH,
            ButtonScale = BUTTON_SCALE,
            ButtonStartingPosition = new(0.5f, (LogoOffsetY * 2f) + LogoSizeY + (BUTTON_SCALE / 2f)),
            DefaultFontAssetName = ASSET_NAME_FONT,
            DefaultFontSize = DEFAULT_FONT_SIZE,
            TextBoxScale = DEFAULT_TEXTBOX_SCALE,
        };

        _startingUI = new(TypedScene, SceneServices, _foregroundLayer, UIProperties);
        _startingUI.IsVisible = true;
        _startingUI.IsEnabled = true;
        AddComponent(_startingUI);

        _playUI = new(TypedScene, SceneServices, _foregroundLayer, UIProperties);
        AddComponent(_playUI);

        _background = new(TypedScene, SceneServices, _backgroundLayer);
        AddComponent(_background);

        _exploreUI = new(TypedScene, SceneServices, _foregroundLayer, UIProperties);
        AddComponent(_exploreUI);

        InitEventHandlers();
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPreComponent();
        
        SceneServices.GetRequired<IFrameExecutor>().SetFrame(_frame);
    }
}