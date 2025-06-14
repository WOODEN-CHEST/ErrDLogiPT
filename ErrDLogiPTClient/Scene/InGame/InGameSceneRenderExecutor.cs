using ErrDLogiPTClient.OS;
using GHEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.InGame;

public class InGameSceneRenderExecutor : SceneComponentBase<InGameScene>
{
    // Private static fields.
    private const string LAYER_NAME_OS = "os";


    // Private fields.
    private readonly IGameOSInstance _osInstance;

    private IGameFrame _frame;
    private ILayer _osLayer;

    // Constructors.
    public InGameSceneRenderExecutor(InGameScene scene, GenericServices services, IGameOSInstance osInstance)
        : base(scene, services)
    {
        _osInstance = osInstance ?? throw new ArgumentNullException(nameof(osInstance));
    }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        _frame = new GHGameFrame();
        _osLayer = new GHLayer(LAYER_NAME_OS);
        _frame.AddLayer(_osLayer);
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPreComponent();
        SceneServices.GetRequired<IFrameExecutor>().SetFrame(_frame);
    }
}