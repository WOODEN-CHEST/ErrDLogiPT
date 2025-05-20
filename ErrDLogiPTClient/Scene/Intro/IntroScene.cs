using ErrDLogiPTClient.Mod;
using GHEngine;
using GHEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.Intro;

public class IntroScene : SceneBase
{
    // Private fields.
    private readonly IGameServices _services;
    private readonly IModManager _modManager;
    private readonly ILogiAssetManager _logiAssetManager;
    
    private readonly IntroFrameExecutor _frameExecutor;


    // Constructors.
    public IntroScene(IGameServices services, IModManager modManager) : base(services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _modManager = modManager ?? throw new ArgumentNullException(nameof(modManager));
        _logiAssetManager = new LogiAssetManager(_services, modManager);
        _frameExecutor = new(this, AssetProvider, Services);

        Components.Add(_frameExecutor);
        Components.Add(new GamePropertiesInitializer(this, AssetProvider, Services));
    }


    // Inherited methods.
    protected override void HandleLoad()
    {
        base.HandleLoad();

        _logiAssetManager.LoadAssetDefinitions();
        _logiAssetManager.SetAsserRootPaths(Array.Empty<string>());

        _frameExecutor.OnLoad();
    }
}