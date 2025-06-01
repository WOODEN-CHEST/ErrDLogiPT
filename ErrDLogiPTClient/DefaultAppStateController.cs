using GHEngine;
using GHEngine.IO;
using GHEngine.Screen;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public class DefaultAppStateController : IAppStateController
{
    // Fields.
    public bool CanSwitchFullScreen { get; set; }
    public bool IsRestartScheduled { get; private set; }


    // Private fields.
    private readonly LogiGame _game;
    private readonly GenericServices _services;



    // Constructors.
    public DefaultAppStateController(LogiGame game, GenericServices services)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }


    // Protected methods,
    protected bool IsFullScreenSwitchTriggered(IUserInput input)
    {
        return input.WereKeysJustPressed(Keys.F11) || (input.WereKeysJustPressed(Keys.Enter) && input.AreKeysDown(Keys.LeftAlt));
    }


    // Private methods.
    private void TrySwitchFullScreenMode()
    {
        IUserInput? Input = _services.Get<IUserInput>();
        if (Input == null)
        {
            return;
        }

        if (IsFullScreenSwitchTriggered(Input))
        {
            IDisplay? Display = _services.Get<IDisplay>();
            if (Display != null)
            {
                Display.FullScreenSize = Display.ScreenSize;
                Display.IsFullScreen = !Display.IsFullScreen;
            }
        }
    }


    // Inherited methods.
    public void Exit()
    {
        _game.Exit();
    }

    public void Restart()
    {
        IsRestartScheduled = true;
        Exit();
    }

    public void Update(IProgramTime time)
    {
        TrySwitchFullScreenMode();
    }
}