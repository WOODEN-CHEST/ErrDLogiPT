using ErrDLogiPTClient.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.InGame;

public class InGameScene : SceneBase
{
    // Private fields.
    private IGameOSInstance _operatingSystem;
    private InGameRenderExecutor _renderExecutor;


    // Constructors.
    public InGameScene(GenericServices globalServices) : base(globalServices) { }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        _renderExecutor = new(this, SceneServices);
        _operatingSystem = null;

        base.HandleLoadPreComponent();
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPreComponent();
    }
}