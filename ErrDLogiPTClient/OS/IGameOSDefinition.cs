using ErrDLogiPTClient.Service;
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
    DateTime ReleaseDate { get; }
    string DefinitionKey { get; }


    // Methods.
    IGameOSInstance CreateInstance(GlobalServices sceneServices);
}