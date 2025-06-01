using ErrDLogiPTClient.Scene.Sound;
using GHEngine.Assets.Def;
using GHEngine.Frame.Animation;
using GHEngine.GameFont;
using GHEngine.IO;
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
            AssetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_BUTTON_FONT));
    }

    public IBasicCheckmark CreateCheckmark()
    {
        throw new NotImplementedException();
    }

    public IBasicDropdownList CreateDropdownList()
    {
        throw new NotImplementedException();
    }

    public IBasicSlider CreateSlider()
    {
        throw new NotImplementedException();
    }
}