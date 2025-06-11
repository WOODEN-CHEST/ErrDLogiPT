using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.Checkmark;
using ErrDLogiPTClient.Scene.UI.Dropdown;
using ErrDLogiPTClient.Scene.UI.Slider;
using ErrDLogiPTClient.Scene.UI.UITextBox;
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
    // Static fields.
    public const string ASSET_NAME_BASIC_BUTTON = "main_button";
    public const string ASSET_NAME_MAIN_FONT = "main";
    public const string ASSET_NAME_BASIC_SLIDER = "main_slider";
    public const string ASSET_NAME_BASIC_DROPDOWN_LIST = "main_dropdown_list";
    public const string ASSET_NAME_BASIC_CHECKMARK = "main_checkmark";
    public const string ASSET_NAME_BASIC_TEXTBOX = "main_textbox";

    public static readonly Color NORMAL_COLOR = Color.White;
    public static readonly Color HOVER_COLOR = new Color(173, 255, 110, 255);
    public static readonly Color CLICK_COLOR = new Color(79, 299, 240, 255);
    public static readonly Color LIST_ELEMENT_COLOR = new Color(180, 180, 180, 255);
    public static readonly Color LIST_ELEMENT_SELECTED_COLOR = new Color(0, 200, 0, 255);
    public static readonly Color LIST_ELEMENT_UNAVAILABLE_COLOR = new Color(100, 100, 100, 255);
    public static readonly Color CHECKMARK_COLOR = new Color(0, 255, 0, 255);
    public static readonly Color TEXTBOX_TEXT_COLOR = Color.Black;

    public static readonly TimeSpan HOVER_FADE_DURATION = TimeSpan.FromSeconds(0.1d);
    public static readonly TimeSpan CLICK_FADE_DURATION = TimeSpan.FromSeconds(0.4d);

    public const float TEXT_SHADOWN_BRIGHTNESS = 0.25f;
    public const float TEXT_SHADOWN_OFFSET = 0.08f;




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
        AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_DROPDOWN_LIST);
        AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_CHECKMARK);
        AssetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_MAIN_FONT);
    }

    public IBasicButton CreateButton()
    {
        ISceneAssetProvider AssetProvider = _sceneServices.GetRequired<ISceneAssetProvider>();

        return new DefaultBasicButton(_sceneServices.GetRequired<ILogiSoundEngine>(),
            _sceneServices.GetRequired<IUserInput>(),
            AssetProvider,
            AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_BUTTON),
            AssetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_MAIN_FONT))
        {
            ButtonColor = NORMAL_COLOR,
            HoverColor = HOVER_COLOR,
            ClickColor = CLICK_COLOR,
            ClickFadeDuration = CLICK_FADE_DURATION,
            HoverFadeDuration = HOVER_FADE_DURATION,

            TextShadowBrightness = TEXT_SHADOWN_BRIGHTNESS,
            ShadowOffset = new(TEXT_SHADOWN_OFFSET)
        };
    }

    public IBasicCheckmark CreateCheckmark()
    {
        ISceneAssetProvider AssetProvider = _sceneServices.GetRequired<ISceneAssetProvider>();

        return new DefaultBasicCheckmark(_sceneServices.GetRequired<IUserInput>(),
            _sceneServices.GetRequired<ILogiSoundEngine>(),
            AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_CHECKMARK),
            AssetProvider)
        {
            CheckmarkColor = CHECKMARK_COLOR,
            HoverColor = HOVER_COLOR,
            ClickColor = CLICK_COLOR,
            NormalColor = NORMAL_COLOR,

            HoverFadeDuration = HOVER_FADE_DURATION,
            ClickFadeDuration = CLICK_FADE_DURATION,
        };
    }

    public IBasicDropdownList<T> CreateDropdownList<T>()
    {
        ISceneAssetProvider AssetProvider = _sceneServices.GetRequired<ISceneAssetProvider>();

        return new DefaultBasicDropdownList<T>(_sceneServices.GetRequired<IUserInput>(),
            AssetProvider,
            () => CreateButton(),
            AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_DROPDOWN_LIST))
        {
            DefaultElementColor = LIST_ELEMENT_COLOR,
            DefaultElementHoverColor = HOVER_COLOR,
            DefaultElementClickColor = CLICK_COLOR,
            DefaultElementSelectedColor = LIST_ELEMENT_SELECTED_COLOR,
            DefaultElementUnavailableColor = LIST_ELEMENT_UNAVAILABLE_COLOR,

            ValueDisplayColor = NORMAL_COLOR,
            ValueDisplayHoverColor = HOVER_COLOR,
            ValueDisplayChangeColor = CLICK_COLOR,

            HoverColorDuration = HOVER_FADE_DURATION,
            ValueChangeColorDuration = CLICK_FADE_DURATION,

            TextShadowBrightness = TEXT_SHADOWN_BRIGHTNESS
        };
    }

    public IBasicSlider CreateSlider()
    {
        ISceneAssetProvider AssetProvider = _sceneServices.GetRequired<ISceneAssetProvider>();

        return new DefaultBasicSlider(_sceneServices.GetRequired<IUserInput>(),
            _sceneServices.GetRequired<ILogiSoundEngine>(),
            AssetProvider,
            AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_SLIDER),
            AssetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_MAIN_FONT))
        {
            NormalColor = NORMAL_COLOR,
            HoverColor = HOVER_COLOR,
            GrabColor = CLICK_COLOR,

            HoverFadeDuration = HOVER_FADE_DURATION,
            GrabFadeDuration = CLICK_FADE_DURATION,

            TrackColor = NORMAL_COLOR,
            HandleColor = NORMAL_COLOR,

            TextShadowBrightness = TEXT_SHADOWN_BRIGHTNESS,
            ShadowOffset = new(TEXT_SHADOWN_OFFSET)
        };
    }

    public IBasicTextBox CreateTextBox()
    {
        ISceneAssetProvider AssetProvider = _sceneServices.GetRequired<ISceneAssetProvider>();

        return new DefaultBasicTextBox(_sceneServices.GetRequired<IUserInput>(),
            AssetProvider,
            AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_TEXTBOX))
        {
            BoxColor = NORMAL_COLOR,
            GlobalTextColor = TEXTBOX_TEXT_COLOR
        };
    }
}