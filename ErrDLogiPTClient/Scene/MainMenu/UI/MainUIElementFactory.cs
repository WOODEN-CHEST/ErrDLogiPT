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

namespace ErrDLogiPTClient.Scene.MainMenu.UI;

public class MainUIElementFactory
{
    // Private static fields.
    private const string ASSET_NAME_BUTTON_BASIC = "main_button";
    private const string ASSET_NAME_BUTTON_FONT = "main";


    // Private fields.
    private readonly ISceneAssetProvider _assetProvider;
    private readonly IUserInput _input;


    // Constructors.
    public MainUIElementFactory(ISceneAssetProvider assetProvider, IUserInput input)
    {
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        _input = input ?? throw new ArgumentNullException(nameof(input));
    }


    // Methods.
    public void LoadAssets()
    {
        _assetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BUTTON_BASIC);
        _assetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_BUTTON_FONT);
    }

    public MainMenuBasicButton CreateButton(float lengthSegments)
    {
        MainMenuBasicButton Button = new(_input,
            _assetProvider, 
            _assetProvider.GetAsset<ISpriteAnimation>(AssetType.Animation, ASSET_NAME_BUTTON_BASIC),
            _assetProvider.GetAsset<GHFontFamily>(AssetType.Font, ASSET_NAME_BUTTON_FONT),
            lengthSegments);

        return Button;
    }
}