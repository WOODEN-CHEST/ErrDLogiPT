using ErrDLogiPTClient.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Service;

public class WrapperServices : DefaultGenericServices
{
    // Methods.
    public void Initialize()
    {
        foreach (var Entry in Services)
        {
           ((IServiceWrapperObject)Entry.Value).InitializeWrapper();
        }
    }

    public void Deinitialize()
    {
        foreach (var Entry in Services)
        {
            ((IServiceWrapperObject)Entry.Value).DeinitializeWrapper();
        }
    }


    // Inherited methods.
    public override T? Set<T>(T? service) where T : class
    {
        if ((service != null) && (service is not IServiceWrapperObject))
        {
            throw new ArgumentException($"The service {service.GetType().FullName} is not a wrapper");
        }
        return base.Set(service);
    }

    protected override bool IsSetServiceValid(object? service)
    {
        return (service == null) || (service is IServiceWrapperObject);
    }
}