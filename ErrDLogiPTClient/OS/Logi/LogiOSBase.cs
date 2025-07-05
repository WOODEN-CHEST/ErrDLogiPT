using ErrDLogiPTClient.Service;
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
    public IGenericServices OSServices { get; private init; }


    // Constructors.
    public LogiOSBase(IGameOSDefinition definition, IGenericServices services)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        OSServices = services ?? throw new ArgumentNullException(nameof(services));
    }


    // Protected methods.
    protected virtual void HandleStart() { }
    protected virtual void HandleShutDown() { }
    protected virtual void HandleRestart() { }


    // Private methods.
    private void InitializeServices()
    {

    }


    // Inherited methods.
    public void Restart()
    {

    }

    public void ShutDown()
    {

    }

    public void Start()
    {
        InitializeServices();
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