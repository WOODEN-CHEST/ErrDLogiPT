using ErrDLogiPTClient.OS;
using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.Dropdown;
using GHEngine;
using GHEngine.Frame;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    // Private static fields.
    private readonly Vector2 BACK_BUTTON_OFFSET = new(0f, 0.1f);
    private readonly Vector2 BACK_BUTTON_POSITION = new(0.5f, 1f);
    private readonly Color SELECT_BUTTON_UNAVAILABLE_COLOR = new(80, 80, 80, 255);


    // Private fields.
    private readonly MainMenuUIProperties _uiProperties;

    private IBasicDropdownList<IGameOSDefinition> _osSelectionDropdown;
    private IBasicButton _backButton;
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
    private void UpdatePositions(IntVector windowSize)
    {
        float AspectRatio = (float)windowSize.X / (float)windowSize.Y;
    }

    private void CreateElements()
    {
        IUIElementFactory Factory = SceneServices.GetRequired<IUIElementFactory>();

        _backButton = Factory.CreateButton();
    }

    private void InitElements()
    {
        CreateElements();
        UpdatePositions(SceneServices.GetRequired<IDisplay>().CurrentWindowSize);
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        InitElements();

        base.HandleLoadPreComponent();
    }
}