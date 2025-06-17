using ErrDLogiPTClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public abstract class ServiceWrapper<T> : IServiceWrapperObject
{
    // Protected fields.
    protected IGenericServices Services { get; private init; }
    protected T ServiceObject { get; private set; }


    // Constructors.
    public ServiceWrapper(IGenericServices services)
    {

    }




    // Inherited methods.
    public void InitializeWrapper()
    {
        throw new NotImplementedException();
    }

    public void DeinitializeWrapper()
    {
        throw new NotImplementedException();
    }
}