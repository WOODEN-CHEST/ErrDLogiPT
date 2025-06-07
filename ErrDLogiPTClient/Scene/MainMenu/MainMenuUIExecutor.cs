using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Scene.UI.Checkmark;
using ErrDLogiPTClient.Scene.UI.Dropdown;
using ErrDLogiPTClient.Scene.UI.Slider;
using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Audio.Source;
using GHEngine.Frame;
using GHEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

    private readonly MainMenuStartingUI _startingUI;


    // Constructors.
    public MainMenuUIExecutor(MainMenuScene scene, GenericServices sceneServices) : base(scene, sceneServices)
    {
        _frame = new GHGameFrame();

        _backgroundLayer = new GHLayer(LAYER_NAME_BACKGROUND);
        _foregroundLayer = new GHLayer(LAYER_NAME_FOREGROUND);
        _frame.AddLayer(_backgroundLayer);
        _frame.AddLayer(_foregroundLayer);

        _startingUI = new(TypedScene, SceneServices, _foregroundLayer);
        AddComponent(_startingUI);
    }


    // Private methods.



    // Inherited methods.
    public override void OnLoad()
    {
        base.OnLoad();

        IUIElementFactory Factory = SceneServices.GetRequired<IUIElementFactory>();
        Factory.LoadAssets();
    }

    public override void OnStart()
    {
        base.OnStart();

        _startingUI.IsVisible = true;
        _startingUI.IsEnabled = true;
        
        SceneServices.GetRequired<IFrameExecutor>().SetFrame(_frame);
    }
}