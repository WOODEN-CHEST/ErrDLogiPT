using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Registry;

public class DefaultItemRegistry<T> : IGameItemRegistry<T>
{
    // Fields.
    public int RegisteredItemCount => throw new NotImplementedException();


    // Private fields.
    private readonly Dictionary<string, T> _registry = new();


    // Inherited methods.
    public bool Get(string key, out T? value)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        return _registry.TryGetValue(key, out value);
    }

    public void Register(string key, T value)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        if (_registry.ContainsKey(key))
        {
            throw new ArgumentException($"Item with key \"{key}\" is already registered.", nameof(key));
        }
        _registry.Add(key, value);
    }

    public bool Unregister(string key)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        return _registry.Remove(key);
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (T Item in _registry.Values)
        {
            yield return Item;
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}