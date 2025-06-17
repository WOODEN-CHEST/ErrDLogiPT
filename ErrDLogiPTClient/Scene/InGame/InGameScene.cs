using ErrDLogiPTClient.OS;
using ErrDLogiPTClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.InGame;

public class InGameScene : SceneBase
{
    // Private fields.
    private readonly InGameOSCreateOptions _osOptions;
    private InGameRenderExecutor _renderExecutor;

    private IGameOSInstance _osInstance;


    // Constructors.
    public InGameScene(GlobalServices globalServices, InGameOSCreateOptions osOptions) : base(globalServices)
    {
        _osOptions = osOptions ?? throw new ArgumentNullException(nameof(osOptions));
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        _osInstance = _osOptions.TargetOS.CreateInstance(SceneServices);

        base.HandleLoadPreComponent();
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPreComponent();
    }
}