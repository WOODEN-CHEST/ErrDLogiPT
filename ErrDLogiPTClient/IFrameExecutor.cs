using GHEngine;
using GHEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public interface IFrameExecutor : IDisposable
{
    // Fields.
    IGameFrame? CurrentFrame { get; }
    IFrameRenderer Renderer { get; }


    // Methods.
    void SetFrame(IGameFrame frame);
    void Render(IProgramTime time);
}