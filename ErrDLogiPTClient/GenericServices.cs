using System;
using System.Collections.Generic;

namespace ErrDLogiPTClient;

/* Look, this is bad, I agree, hidden dependencies and such, but I couldn't think of anything better. */

/// <summary>
/// An object which stores various services such as input, display, render objects and so on.
/// </summary>
public class GenericServices
{
    // Fields.
    public int ServiceCount => _services.Count;


    // Private fields.
    private readonly Dictionary<Type, object> _services = new();


    // Methods.
    public T? Get<T>() where T : class
    {
        Type TargetType = typeof(T);
        if (_services.TryGetValue(TargetType, out object? Service) && (Service is T CastService))
        {
            return CastService;
        }
        return null;
    }

    public T GetRequired<T>() where T : class
    {
        T? Service = Get<T>();
        return Service == null ? throw new ServiceException($"Missing required service of type {typeof(T).FullName}") : Service;
    }

    public T? Set<T>(T? service) where T : class
    {
        Type TargetType = typeof(T);
        T? OldService = Get<T>();

        if (service == null)
        {
            _services.Remove(TargetType);
        }
        else
        {
            _services[TargetType] = service;
        }
        return OldService;
    }
}