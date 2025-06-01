using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI;
using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Audio.Source;
using GHEngine.Frame;
using GHEngine.IO;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuUIExecutor : SceneComponentBase<MainMenuScene>
{
    // Private static fields.
    private const string LAYER_NAME_BACKGROUND = "background";
    private const string LAYER_NAME_FOREGROUND = "foreground";


    // Private fields.
    private readonly IGameFrame _frame;
    private readonly ILayer _backgroundLayer;
    private readonly ILayer _foregroundLayer;

    private IBasicButton _button;


    // Constructors.
    public MainMenuUIExecutor(MainMenuScene scene, GenericServices sceneServices) : base(scene, sceneServices)
    {
        _frame = new GHGameFrame();

        _backgroundLayer = new GHLayer(LAYER_NAME_BACKGROUND);
        _foregroundLayer = new GHLayer(LAYER_NAME_FOREGROUND);
        _frame.AddLayer(_backgroundLayer);
        _frame.AddLayer(_foregroundLayer);
    }


    // Private methods.



    // Inherited methods.
    public override void OnLoad()
    {
        base.OnLoad();

        IUIElementFactory Factory = SceneServices.GetRequired<IUIElementFactory>();
        Factory.LoadAssets();

        _button = Factory.CreateButton();
        _button.Length = 4f;
        _button.Position = new(0.5f, 0.5f);
        _button.Scale = 0.1f;
        _button.Text = "Hello World!";
        
        _button.ClickSounds = new IPreSampledSound[] 
        {
            SceneServices.GetRequired<ISceneAssetProvider>().GetAsset<IPreSampledSound>(AssetType.Sound, "test") 
        };
        _button.Volume = 0.25f;

        _button.Scroll += (sender, args) =>
        {
            if (args.ScrollAmount > 0)
            {
                _button.Scale += 0.1f;
            }
            else
            {
                _button.Scale -= 0.1f;
            }
        };
    }

    public override void OnStart()
    {
        base.OnStart();

        SceneServices.GetRequired<IFrameExecutor>().SetFrame(_frame);

        _button.Initialize();
        _button.Scale = 0.1f;
        _foregroundLayer.AddItem(_button);
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);
            
        _button.Update(time);
    }
}