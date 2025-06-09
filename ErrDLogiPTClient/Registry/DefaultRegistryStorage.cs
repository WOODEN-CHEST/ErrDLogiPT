using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Registry;

public class DefaultRegistryStorage : IGameRegistryStorage
{
    // Private fields.
    private readonly Dictionary<Type, object> _registries = new();


    // Inherited methods.
    public IGameItemRegistry<T>? GetRegistry<T>()
    {
        Type TargetType = typeof(T);
        if (_registries.TryGetValue(TargetType, out var Registry))
        {
            return (IGameItemRegistry<T>)Registry;
        }
        return null;
    }

    public bool RemoveRegistry<T>()
    {
        Type TargetType = typeof(T);
        return _registries.Remove(TargetType);
    }

    public void SetRegistry<T>(IGameItemRegistry<T> registry)
    {
        ArgumentNullException.ThrowIfNull(registry, nameof(registry));
        Type TargetType = typeof(T);

        if (_registries.ContainsKey(TargetType))
        {
            throw new ArgumentException($"Registry with type \"{TargetType.FullName}\" already exists!");
        }

        _registries[TargetType] = registry;
    }
}
