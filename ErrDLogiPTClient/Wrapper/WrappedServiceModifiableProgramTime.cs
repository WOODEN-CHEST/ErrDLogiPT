using ErrDLogiPTClient.Service;
using GHEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrappedServiceModifiableProgramTime : ServiceWrapper<IModifiableProgramTime>, IModifiableProgramTime
{
    // Fields.
    public TimeSpan PassedTime
    {
        get => ServiceObject.PassedTime;
        set => ServiceObject.PassedTime = value;
    }

    public TimeSpan TotalTime
    {
        get => ServiceObject.TotalTime;
        set => ServiceObject.TotalTime = value;
    }


    // Constructors.
    public WrappedServiceModifiableProgramTime(IGenericServices services) : base(services) { }
}