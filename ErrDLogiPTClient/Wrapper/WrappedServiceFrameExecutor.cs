using ErrDLogiPTClient.Service;
using GHEngine;
using GHEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrappedServiceFrameExecutor : ServiceWrapper<IFrameExecutor>, IFrameExecutor
{
    // Fields.
    public IGameFrame? CurrentFrame => ServiceObject.CurrentFrame;

    public IFrameRenderer Renderer => ServiceObject.Renderer;


    // Constructors.
    public WrappedServiceFrameExecutor(IGenericServices services) : base(services) { }


    // Inherited methods.
    public void Dispose() { }

    public void Render(IProgramTime time)
    {
        ServiceObject.Render(time);
    }

    public void SetFrame(IGameFrame? frame)
    {
        ServiceObject?.SetFrame(frame);
    }
}