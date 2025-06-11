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

public class DefaultFullScreenToggler : IFulLScreenToggler
{
    // Fields.
    public bool CanSwitchFullScreen { get; set; } = true;


    // Private fields.
    private readonly GenericServices _services;

    // Constructors.
    public DefaultFullScreenToggler(GenericServices services)
    {
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
        if (!CanSwitchFullScreen)
        {
            return;
        }

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
                Display.IsFullScreen = !Display.IsFullScreen;
            }
        }
    }


    // Inherited methods.
    public void Update(IProgramTime time)
    {
        TrySwitchFullScreenMode();
    }

    public void RestoreFullScreenSize()
    {
        IDisplay Display = _services.GetRequired<IDisplay>();
        Display.FullScreenSize = Display.ScreenSize;
    }
}