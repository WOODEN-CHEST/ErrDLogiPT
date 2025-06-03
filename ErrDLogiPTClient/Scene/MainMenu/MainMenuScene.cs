using System;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.MainMenu;

public class MainMenuScene : SceneBase
{
    // Fields.
    


    // Private fields.
    private readonly MainMenuUIExecutor _uiExecutor;


    // Constructors.
    public MainMenuScene(GenericServices services) : base(services)
    {
        _uiExecutor = new(this, SceneServices);
        AddComponent(_uiExecutor);
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();
    }
}