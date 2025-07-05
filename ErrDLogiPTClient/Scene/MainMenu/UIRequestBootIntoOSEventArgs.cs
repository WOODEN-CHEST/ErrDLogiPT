using ErrDLogiPTClient.Scene.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class UIRequestBootIntoOSEventArgs : CancellableEventBase
{
    // Fields.
    public InGameOSCreateOptions Options
    {
        get => _options;
        set => _options = value ?? throw new ArgumentNullException(nameof(value));
    }

    // Private fields.
    private InGameOSCreateOptions _options;


    // Constructors.
    public UIRequestBootIntoOSEventArgs(InGameOSCreateOptions options)
    {
        Options = options;
    }
}