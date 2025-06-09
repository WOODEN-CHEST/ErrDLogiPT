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
    private IGameFrame _frame;
    private ILayer _backgroundLayer;
    private ILayer _foregroundLayer;

    private MainMenuStartingUI _startingUI;


    // Constructors.
    public MainMenuUIExecutor(MainMenuScene scene, GenericServices sceneServices) : base(scene, sceneServices) { }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        _frame = new GHGameFrame();

        _backgroundLayer = new GHLayer(LAYER_NAME_BACKGROUND);
        _foregroundLayer = new GHLayer(LAYER_NAME_FOREGROUND);
        _frame.AddLayer(_backgroundLayer);
        _frame.AddLayer(_foregroundLayer);

        IUIElementFactory Factory = SceneServices.GetRequired<IUIElementFactory>();
        Factory.LoadAssets();

        _startingUI = new(TypedScene, SceneServices, _foregroundLayer);
        _startingUI.IsVisible = true;
        _startingUI.IsEnabled = true;
        AddComponent(_startingUI);
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPreComponent();
        
        
        SceneServices.GetRequired<IFrameExecutor>().SetFrame(_frame);
    }
}