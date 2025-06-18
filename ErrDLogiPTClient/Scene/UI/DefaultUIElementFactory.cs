using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.Checkmark;
using ErrDLogiPTClient.Scene.UI.Dropdown;
using ErrDLogiPTClient.Scene.UI.Slider;
using ErrDLogiPTClient.Scene.UI.UITextBox;
using ErrDLogiPTClient.Service;
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

    public const float TEXT_SHADOW_BRIGHTNESS = 0.25f;
    public const float TEXT_SHADOWN_OFFSET = 0.08f;

    public static readonly TimeSpan HOVER_FADE_DURATION = TimeSpan.FromSeconds(0.1d);
    public static readonly TimeSpan CLICK_FADE_DURATION = TimeSpan.FromSeconds(0.4d);


    // Fields.
    public Color NormalColor => Color.White;
    public Color HoverColor => new Color(173, 255, 110, 255);
    public Color ClickColor => new Color(79, 299, 240, 255);
    public Color ListElementColor => new Color(180, 180, 180, 255);
    public Color ListElementSelectedColor => new Color(0, 200, 0, 255);
    public Color UnavailableColor => new Color(100, 100, 100, 255);
    public Color CheckmarkColor => new Color(0, 255, 0, 255);
    public Color TextboxTextColor => Color.Black;


    // Private fields.
    private readonly IGenericServices _sceneServices;


    // Constructors.
    public DefaultUIElementFactory(IGenericServices sceneServices)
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
            ButtonColor = NormalColor,
            HoverColor = HoverColor,
            ClickColor = ClickColor,
            ClickFadeDuration = CLICK_FADE_DURATION,
            HoverFadeDuration = HOVER_FADE_DURATION,

            TextShadowBrightness = TEXT_SHADOW_BRIGHTNESS,
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
            CheckmarkColor = CheckmarkColor,
            HoverColor = HoverColor,
            ClickColor = ClickColor,
            NormalColor = NormalColor,

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
            DefaultElementColor = ListElementColor,
            DefaultElementHoverColor = HoverColor,
            DefaultElementClickColor = ClickColor,
            DefaultElementSelectedColor = ListElementSelectedColor,
            DefaultElementUnavailableColor = UnavailableColor,

            ValueDisplayColor = NormalColor,
            ValueDisplayHoverColor = HoverColor,
            ValueDisplayChangeColor = ClickColor,

            HoverColorDuration = HOVER_FADE_DURATION,
            ValueChangeColorDuration = CLICK_FADE_DURATION,

            TextShadowBrightness = TEXT_SHADOW_BRIGHTNESS,
            ShadowOffset = new(TEXT_SHADOWN_OFFSET)
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
            NormalColor = NormalColor,
            HoverColor = HoverColor,
            GrabColor = ClickColor,

            HoverFadeDuration = HOVER_FADE_DURATION,
            GrabFadeDuration = CLICK_FADE_DURATION,

            TrackColor = NormalColor,
            HandleColor = NormalColor,

            TextShadowBrightness = TEXT_SHADOW_BRIGHTNESS,
            ShadowOffset = new(TEXT_SHADOWN_OFFSET)
        };
    }

    public IBasicTextBox CreateTextBox()
    {
        ISceneAssetProvider AssetProvider = _sceneServices.GetRequired<ISceneAssetProvider>();

        return new DefaultBasicTextBox(_sceneServices.GetRequired<IUserInput>(),
            AssetProvider,
            AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_TEXTBOX),
            AssetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_MAIN_FONT))
        {
            BoxColor = NormalColor,
            GlobalTextColor = TextboxTextColor
        };
    }
}