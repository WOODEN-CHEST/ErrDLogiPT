using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

public class GamePropertiesInitializer : SceneComponentBase<IntroScene>
{
    // Constructors.
    public GamePropertiesInitializer(IntroScene scene,
        ISceneAssetProvider assetProvider,
        IGameServices services)
        : base(scene, assetProvider, services) { }


    // Inherited methods.
    public override void OnStart()
    {
        base.OnStart();

        Services.Display.IsUserResizingAllowed = true;
        Services.Input.IsMouseVisible = true;
        Services.Input.IsAltF4Allowed = true;
    }
}