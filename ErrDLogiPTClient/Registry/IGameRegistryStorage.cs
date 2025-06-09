using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Registry;

public interface IGameRegistryStorage
{
    // Methods.
    IGameItemRegistry<T>? GetRegistry<T>();
    void SetRegistry<T>(IGameItemRegistry<T> registry);
    bool RemoveRegistry<T>();
}