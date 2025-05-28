using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene.MainMenu.UI;
using GHEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuScene : SceneBase
{
    // Private fields.
    private readonly MainMenuUIExecutor _uiExecutor;


    // Constructors.
    public MainMenuScene(GameServices services) : base(services)
    {
        _uiExecutor = new(this, Services.FrameExecutor, AssetProvider, Services.Input, Services.SoundEngine);
        AddComponent(_uiExecutor);
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();
    }
}