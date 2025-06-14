using ErrDLogiPTClient.Scene.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuEnterOSEventArgs : EventArgs
{
    // Fields.
    public InGameOSCreateOptions Options { get; }


    // Constructors.
    public MainMenuEnterOSEventArgs(InGameOSCreateOptions options)
    {
        Options = options;
    }
}