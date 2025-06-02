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

    private IBasicDropdownList<int> _element;


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

        _element = Factory.CreateDropdownList<int>();
        _element.Position = new(0.5f, 0.2f);
        _element.Length = 4f;
        _element.Scale = 0.2f;

        DropdownListElement<int>[] Elements = new DropdownListElement<int>[]
        {
            new(1.ToString(), 1),
            new(2.ToString(), 2),
            new(3.ToString(), 3),
            new(4.ToString(), 4),
            new(5.ToString(), 5),
            new(6.ToString(), 6),
        };
        _element.SetElements(Elements);
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
    }
}