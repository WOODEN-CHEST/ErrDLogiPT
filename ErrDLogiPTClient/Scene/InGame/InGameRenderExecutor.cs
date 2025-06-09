using GHEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Scene.InGame;

public class InGameRenderExecutor : SceneComponentBase<InGameScene>
{
    // Private fields.
    private IGameFrame _frame;
    private ILayer _osLayer;


    // Constructors.
    public InGameRenderExecutor(InGameScene scene, GenericServices services) : base(scene, services) { }


    // Inherited methods.
    protected override void HandleLoadPreComponent()
    {
        base.HandleLoadPreComponent();

        _frame = new GHGameFrame();
        _osLayer = new GHLayer("operating system");
        _frame.AddLayer(_osLayer);
    }

    protected override void HandleStartPreComponent()
    {
        base.HandleStartPreComponent();

        SceneServices.GetRequired<IFrameExecutor>().SetFrame(_frame);
    }
}