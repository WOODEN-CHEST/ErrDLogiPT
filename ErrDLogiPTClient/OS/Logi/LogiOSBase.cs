using GHEngine;
using GHEngine.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.OS.Logi;

public abstract class LogiOSBase : IGameOSInstance
{
    // Fields.
    public IGameOSDefinition Definition { get; private init; }
    public bool IsVisible { get; set; } = true;
    public GenericServices Services { get; private init; }


    // Constructors.
    public LogiOSBase(IGameOSDefinition definition, GenericServices services)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }


    // Protected methods.
    protected virtual void HandleStart() { }
    protected virtual void HandleShutDown() { }
    protected virtual void HandleRestart() { }


    // Private methods.


    // Inherited methods.
    public void Restart()
    {

    }

    public void ShutDown()
    {

    }

    public void Start()
    {

    }

    public void Update(IProgramTime time)
    {

    }

    public void Render(IRenderer renderer, IProgramTime time)
    {
        if (IsVisible)
        {
            return;
        }
    }
}