using GHEngine;
using GHEngine.Frame;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

public class DefaultFrameExecutor : IFrameExecutor
{
    // Fields.
    public IGameFrame? CurrentFrame { get; private set; }
    public IFrameRenderer Renderer { get; private init; }


    // Constructors.
    public DefaultFrameExecutor(GraphicsDevice device, IDisplay display)
    {
        Renderer = GHRenderer.Create(device, display);
        Renderer.ScreenColor = Color.Black;
    }


    // Inherited methods,
    public void Dispose()
    {
        Renderer.Dispose();
    }

    public void Render(IProgramTime time)
    {
        if (CurrentFrame != null)
        {
            Renderer.RenderFrame(CurrentFrame, time);
        }
    }

    public void SetFrame(IGameFrame? frame)
    {
        CurrentFrame = frame;
    }
}