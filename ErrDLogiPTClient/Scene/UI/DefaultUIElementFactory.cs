using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI.Button;
using ErrDLogiPTClient.Scene.UI.Checkmark;
using ErrDLogiPTClient.Scene.UI.Dropdown;
using ErrDLogiPTClient.Scene.UI.Slider;
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
    private const string ASSET_NAME_BASIC_DROPDOWN_LIST = "main_dropdown_list";
    private const string ASSET_NAME_BASIC_CHECKMARK = "main_checkmark";

    private static readonly Color NORMAL_COLOR = Color.White;
    private static readonly Color HOVER_COLOR = new Color(173, 255, 110, 255);
    private static readonly Color CLICK_COLOR = new Color(79, 299, 240, 255);
    private static readonly Color LIST_ELEMENT_COLOR = new Color(180, 180, 180, 255);
    private static readonly Color LIST_ELEMENT_SELECTED_COLOR = new Color(0, 200, 0, 255);
    private static readonly Color LIST_ELEMENT_UNAVAILABLE_COLOR = new Color(100, 100, 100, 255);
    private static readonly Color CHECKMARK_COLOR = new Color(0, 255, 0, 255);

    private static readonly TimeSpan HOVER_FADE_DURATION = TimeSpan.FromSeconds(0.1d);
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
        AssetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BASIC_DROPDOWN_LIST);
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
            HoverColor = HOVER_COLOR,
            ClickColor = CLICK_COLOR,
            ClickFadeDuration = CLICK_FADE_DURATION,
            HoverFadeDuration = HOVER_FADE_DURATION
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
            ClickFadeDuration = CLICK_FADE_DURATION
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
        };
    }

    public IBasicSlider CreateSlider()
    {
        throw new NotImplementedException();
    }
}