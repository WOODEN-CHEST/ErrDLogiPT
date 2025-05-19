using ErrDLogiPTClient.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

internal class IntroScene : SceneBase
{
    // Private fields.
    private readonly IGameServices _services;
    private readonly IModManager _modManager;


    // Constructors.
    public IntroScene(IGameServices services, IModManager modManager) : base(services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _modManager = modManager ?? throw new ArgumentNullException(nameof(modManager));
    }


    // Private methods.
}