using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Registry;

public interface IGameItemRegistry<T> : IEnumerable<T>
{
    // Fields.
    int RegisteredItemCount { get; }


    // Methods.
    bool Get(string key, out T? value);
    void Register(string key, T value);
    bool Unregister(string key);
}