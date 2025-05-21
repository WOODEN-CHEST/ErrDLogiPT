using ErrDLogiPTClient.Mod;
using GHEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuScene : SceneBase
{
    // Constructors.
    public MainMenuScene(IGameServices services, IModManager modManager, ILogiAssetManager assetManager)
        : base(services)
    {
        Components.Add(new GameHotkeyExecutor(this, AssetProvider, Services));
    }


    // Inherited methods.
    protected override void HandleLoad()
    {
        base.HandleLoad();

        Task.Delay(TimeSpan.FromSeconds(4)).Wait();
    }

    public override void OnStart()
    {
        base.OnStart();

        Services.FrameExecutor.SetFrame(new GHGameFrame());
    }
}