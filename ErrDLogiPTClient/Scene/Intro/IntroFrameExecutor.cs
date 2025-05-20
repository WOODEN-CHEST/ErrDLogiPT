using GHEngine;
using GHEngine.Assets.Def;
using GHEngine.Frame;
using GHEngine.Frame.Animation;
using GHEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

public class IntroFrameExecutor : SceneComponentBase<IntroScene>
{
    // Private static fields.
    private const string LOGO_LAYER_NAME = "main";


    // Private fields.
    private readonly IntroLogoDisplayer _logoDisplayer;
    private readonly IGameFrame _frame;


    // Constructors.
    public IntroFrameExecutor(IntroScene scene, 
        ISceneAssetProvider assetProvider,
        IGameServices services)
        : base(scene, assetProvider, services)
    {
        _frame = new GHGameFrame();
        _frame.AddLayer(new GHLayer(LOGO_LAYER_NAME));
        _logoDisplayer = new(Scene, AssetProvider, Services, _frame.GetLayer(LOGO_LAYER_NAME)!);

        SubComponents.Add(_logoDisplayer);
    }


    // Inherited methods.
    public override void OnLoad()
    {
        Services.FrameExecutor.SetFrame(_frame);
        base.OnLoad();
    }
}