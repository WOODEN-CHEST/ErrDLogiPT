using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public interface IServiceWrapperObject
{
    // Methods.
    void InitializeWrapper();
    void DeinitializeWrapper();
}