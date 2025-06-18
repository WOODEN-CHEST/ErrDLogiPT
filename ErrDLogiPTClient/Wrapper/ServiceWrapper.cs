using ErrDLogiPTClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public abstract class ServiceWrapper<T> : IServiceWrapperObject where T : class
{
    // Protected fields.
    protected IGenericServices Services { get; private init; }
    protected T ServiceObject { get; private set; }


    // Constructors.
    public ServiceWrapper(IGenericServices services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }


    // Protected methods.
    protected virtual void InitService(T service) { }
    protected virtual void DeinitService(T service) { }


    // Private methods.
    private void OnServiceChangeEvent(object? sender, GenericServicesChangeEventArgs args)
    {
        args.AddSuccessAction(() => 
        {
            DeinitService(ServiceObject);
            UpdateServiceObject();
        });
    }

    protected void UpdateServiceObject()
    {
        ServiceObject = Services.GetRequired<T>();
        InitService(ServiceObject);
    }


    // Inherited methods.
    public void InitializeWrapper()
    {
        Services.ServiceChange += OnServiceChangeEvent;
        UpdateServiceObject();
    }

    public void DeinitializeWrapper()
    {
        Services.ServiceChange -= OnServiceChangeEvent;
        DeinitService(ServiceObject);
    }
}