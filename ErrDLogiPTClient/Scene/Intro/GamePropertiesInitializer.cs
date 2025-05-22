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
    // Private fields.
    private readonly 


    // Constructors.
    public GamePropertiesInitializer(IntroScene scene, IDisplay display, IUserInput input)
        : base(scene) { }


    // Inherited methods.
    public override void OnStart()
    {
        base.OnStart();

        Services.Display.IsUserResizingAllowed = true;
        Services.Input.IsMouseVisible = true;
        Services.Input.IsAltF4Allowed = true;
    }
}