using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.Dropdown;
using GHEngine.Assets.Def;
using GHEngine.Frame.Animation;
using GHEngine.GameFont;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.UI;

public class DefaultUIElementFactory : IUIElementFactory
{
    // Private static fields.
    private const string ASSET_NAME_BASIC_BUTTON = "main_button";
    private const string ASSET_NAME_BUTTON_FONT = "main";
    private const string ASSET_NAME_BASIC_SLIDER = "main_slider";
    private const string ASSET_NAME_BASIC_DORPDOWN_LIST = "main_dropdown_list";
    private const string ASSET_NAME_BASIC_CHECKMARK = "main_checkmark";

    private static readonly Color NORMAL_COLOR = Color.White;
    private static readonly Color HIGHLIGHT_COLOR = new Color(173, 255, 110, 255);
    private static readonly Color CLICK_COLOR = new Color(79, 299, 240, 255);
    private static readonly Color LIST_ELEMENT_COLOR = new Color(180, 180, 180, 255);
    private static readonly Color LIST_ELEMENT_SELECTED_COLOR = new Color(0, 200, 0, 255);
    private static readonly Color LIST_ELEMENT_UNAVAILABLE_COLOR = new Color(100, 100, 100, 255);

    private static readonly TimeSpan HIGLIGHT_FADE_DURATION = TimeSpan.FromSeconds(0.1d);
    private static readonly TimeSpan CLICK_FADE_DURATION = TimeSpan.FromSeconds(0.4d);




    // Private fields.
    private readonly GenericServices _sceneServices;


    // Constructors.
    public DefaultUIElementFactory(GenericServices sceneServices)
    {
        _sceneServices = sceneServices ?? throw new ArgumentNullException(nameof(sceneServices));
    }


    // Methods.
    public void LoadAssets()
    {
        ISceneAssetProvider AssetProvider = _sceneServices.GetRequired<ISceneAssetProvider>();

        AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_BUTTON);
        AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_SLIDER);
        AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_DORPDOWN_LIST);
        AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_CHECKMARK);
        AssetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_BUTTON_FONT);
    }

    public IBasicButton CreateButton()
    {
        ISceneAssetProvider AssetProvider = _sceneServices.GetRequired<ISceneAssetProvider>();

        return new DefaultBasicButton(_sceneServices.GetRequired<ILogiSoundEngine>(),
            _sceneServices.GetRequired<IUserInput>(),
            AssetProvider,
            AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_BUTTON),
            AssetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_BUTTON_FONT))
        {
            ButtonColor = NORMAL_COLOR,
            HighlightColor = HIGHLIGHT_COLOR,
            ClickColor = CLICK_COLOR,
            ClickFadeDuration = CLICK_FADE_DURATION,
            HoverFadeDuration = HIGLIGHT_FADE_DURATION
        };
    }

    public IBasicCheckmark CreateCheckmark()
    {
        throw new NotImplementedException();
    }

    public IBasicDropdownList<T> CreateDropdownList<T>()
    {
        return new DefaultBasicDropdownList<T>(_sceneServices.GetRequired<IUserInput>(),
            () => CreateButton())
        {
            DefaultElementColor = LIST_ELEMENT_COLOR,
            DefaultElementHoverColor = HIGHLIGHT_COLOR,
            DefaultElementClickColor = CLICK_COLOR,
            DefaultElementSelectedColor = LIST_ELEMENT_SELECTED_COLOR,
            DefaultElementUnavailableColor = LIST_ELEMENT_UNAVAILABLE_COLOR,

            HoverColorDuration = HIGLIGHT_FADE_DURATION,
            ValueChangeColorDuration = CLICK_FADE_DURATION,
        };
    }

    public IBasicSlider CreateSlider()
    {
        throw new NotImplementedException();
    }
}