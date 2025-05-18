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


    // Private fields.
    private readonly IFrameRenderer _renderer;
    private readonly Game _game;


    // Constructors.
    public DefaultFrameExecutor(GraphicsDevice device, IDisplay display)
    {
        _renderer = GHRenderer.Create(device, display);
    }


    // Inherited methods,
    public void Dispose()
    {
        _renderer.Dispose();
    }

    public void Render(IProgramTime time)
    {
        if (CurrentFrame != null)
        {
            _renderer.RenderFrame(CurrentFrame, time);
        }
    }

    public void SetFrame(IGameFrame frame)
    {
        CurrentFrame = frame ?? throw new ArgumentNullException(nameof(frame));
    }
}