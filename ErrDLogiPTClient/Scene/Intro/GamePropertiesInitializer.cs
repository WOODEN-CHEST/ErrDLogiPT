using GHEngine.IO;
using GHEngine.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

public class GamePropertiesInitializer : SceneComponentBase<IntroScene>
{
    // Constructors.
    public GamePropertiesInitializer(IntroScene scene, GenericServices sceneServices) : base(scene, sceneServices) { }

    // Inherited methods.
    public override void OnStart()
    {
        base.OnStart();

        IDisplay Display = SceneServices.GetRequired<IDisplay>();
        IUserInput Input = SceneServices.GetRequired<IUserInput>();

        Display.IsUserResizingAllowed = true;
        Input.IsMouseVisible = true;
        Input.IsAltF4Allowed = true;
    }
}