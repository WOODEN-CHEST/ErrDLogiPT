using ErrDLogiPTClient.Service;
using System;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuScene : SceneBase
{
    // Private fields.
    private MainMenuUIExecutor _uiExecutor;


    // Constructors.
    public MainMenuScene(GlobalServices services) : base(services) { }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        _uiExecutor = new(this, SceneServices);
        AddComponent(_uiExecutor);
    }
}