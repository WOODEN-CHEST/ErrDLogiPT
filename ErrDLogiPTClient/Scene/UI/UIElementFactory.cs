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

public class UIElementFactory
{
    // Private static fields.
    private const string ASSET_NAME_BUTTON_BASIC = "main_button";
    private const string ASSET_NAME_BUTTON_FONT = "main";


    // Private fields.
    private readonly ISceneAssetProvider _assetProvider;
    private readonly IUserInput _input;
    private readonly ILogiSoundEngine _soundEngine;


    // Constructors.
    public UIElementFactory(ISceneAssetProvider assetProvider, IUserInput input, ILogiSoundEngine soundEngine)
    {
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _soundEngine = soundEngine ?? throw new ArgumentNullException(nameof(soundEngine));
    }


    // Methods.
    public void LoadAssets()
    {
        _assetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BUTTON_BASIC);
        _assetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_BUTTON_FONT);
    }

    public UIBasicButton CreateButton(float lengthSegments)
    {
        UIBasicButton Button = new(_soundEngine,
            _input,
            _assetProvider,
            _assetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BUTTON_BASIC),
            _assetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_BUTTON_FONT))
        {
            Length = lengthSegments
        };

        return Button;
    }
}