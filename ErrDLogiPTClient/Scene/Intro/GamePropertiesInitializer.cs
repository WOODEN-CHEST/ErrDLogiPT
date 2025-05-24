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
    private readonly IDisplay _display;
    private readonly IUserInput _userInput;


    // Constructors.
    public GamePropertiesInitializer(IntroScene scene, IDisplay display, IUserInput input)
        : base(scene)
    {
        _display = display ?? throw new ArgumentNullException(nameof(display));
        _userInput = input ?? throw new ArgumentNullException(nameof(input));
    }


    // Inherited methods.
    public override void OnStart()
    {
        base.OnStart();

        _display.IsUserResizingAllowed = true;
        _userInput.IsMouseVisible = true;
        _userInput.IsAltF4Allowed = true;
    }
}