using ErrDLogiPTClient.Registry;
using ErrDLogiPTClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrappedServiceGameRegistryStorage : ServiceWrapper<IGameRegistryStorage>, IGameRegistryStorage
{
    // Constructors.
    public WrappedServiceGameRegistryStorage(IGenericServices services) : base(services) { }


    // Inherited methods.
    public IGameItemRegistry<T>? GetRegistry<T>()
    {
        return ServiceObject.GetRegistry<T>();
    }

    public bool RemoveRegistry<T>()
    {
        return ServiceObject.RemoveRegistry<T>();
    }

    public void SetRegistry<T>(IGameItemRegistry<T> registry)
    {
        ServiceObject.SetRegistry<T>(registry);
    }
}