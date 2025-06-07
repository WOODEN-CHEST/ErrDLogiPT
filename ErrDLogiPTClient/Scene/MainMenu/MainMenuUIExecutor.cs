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

    private IBasicSlider _element;
    

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

        _element = Factory.CreateSlider();
        _element.Position = new Vector2(0.5f, 0.5f);
        _element.Scale = 0.025f;
        _element.Length = 64f;
        _element.Orientation = SliderOrientation.Vertical;
        _element.ValueDisplayProvider = (factor) =>
        {
            return factor.ToString("0.00", CultureInfo.InvariantCulture);
        };

        //_element.Step = 1d / (4d - 1d);
    }

    public override void OnStart()
    {
        base.OnStart();

        SceneServices.GetRequired<IFrameExecutor>().SetFrame(_frame);

        _element.Initialize();
        _foregroundLayer.AddItem(_element);
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);

        _element.Update(time);

        IUserInput Input = SceneServices.GetRequired<IUserInput>();
        if (Input.WereKeysJustPressed(Keys.Up))
        {
            _element.Orientation = SliderOrientation.Horizontal;
        }
        else if (Input.WereKeysJustPressed(Keys.Down))
        {
            _element.Orientation = SliderOrientation.Vertical;
        }
    }
}