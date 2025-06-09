using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.OS;

public interface IGameOSDefinition
{
    // Fields.
    string Name { get; }
    string Description { get; }
    DateTime ReleaseData { get; }


    // Methods.
    IGameOS CreateInstance();
}