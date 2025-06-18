using ErrDLogiPTClient.OS;
using ErrDLogiPTClient.OS.Logi.LogiXD;
using ErrDLogiPTClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Registry;

public class RegistryInitializer
{
    // Methods.
    public IGameRegistryStorage CreateRegistryStorage()
    {
        IGameRegistryStorage RegistryStorage = new DefaultRegistryStorage();
        RegistryStorage.SetRegistry<IGameOSDefinition>(GetOSRegistry());
        return RegistryStorage;
    }


    // Private methods.
    private IGameItemRegistry<IGameOSDefinition> GetOSRegistry()
    {
        IGameItemRegistry<IGameOSDefinition> Registry = new DefaultItemRegistry<IGameOSDefinition>();

        IGameOSDefinition LogiXDDefinition = new LogiXDDefinition();
        Registry.Register(LogiXDDefinition.DefinitionKey, LogiXDDefinition);

        return Registry;
    }
}