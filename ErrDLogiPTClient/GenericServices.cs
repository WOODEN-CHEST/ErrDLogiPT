using System;
using System.Collections.Generic;
using System.Linq;

namespace ErrDLogiPTClient;

/* Look, this is bad, I agree, hidden dependencies and such, but I couldn't think of anything better. */

/// <summary>
/// An object which stores various services such as input, display, render objects and so on.
/// </summary>
public class GenericServices
{
    // Fields.
    public int ServiceCount => _services.Count;

    /// <summary>
    /// Event which is fired before a service is set.
    /// </summary>
    public event EventHandler<GenericServicesChangeEventArgs>? ServiceChange;


    // Private fields.
    private readonly Dictionary<Type, object> _services = new();


    // Methods.

    /// <summary>
    /// Gets a service by its type.
    /// </summary>
    /// <typeparam name="T">The type of the service, usually and interface type.</typeparam>
    /// <returns>The service if one exists, otherwise <c>null</c>.</returns>
    public T? Get<T>() where T : class
    {
        Type TargetType = typeof(T);
        if (_services.TryGetValue(TargetType, out object? Service) && (Service is T CastService))
        {
            return CastService;
        }
        return null;
    }

    /// <summary>
    /// Gets a service by its type. Throws an exception if no such service is found.
    /// </summary>
    /// <typeparam name="T">The type of the service, usually and interface type.</typeparam>
    /// <returns>The service.</returns>
    /// <exception cref="ServiceException"></exception>
    public T GetRequired<T>() where T : class
    {
        T? Service = Get<T>();
        return Service == null ? throw new ServiceException($"Missing required service of type {typeof(T).FullName}") : Service;
    }

    /// <summary>
    /// Sets a service by its type. 
    /// </summary>
    /// <typeparam name="T">The type of the service. It must be set to be an interface or base class instead
    /// of a specific service implementation type. This is because the service, when searched by a get method,
    /// searches based on the provided T type argument.</typeparam>
    /// <param name="service">The service to set.</param>
    /// <returns>The old service which is replaced by the new one passed to this function, or <c>null</c>
    /// if there was no old service.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T? Set<T>(T? service) where T : class
    {
        Type TargetType = typeof(T);
        T? OldService = Get<T>();
        if ((service == null) && (OldService == null))
        {
            return null;
        }

        GenericServicesChangeEventArgs EventArgs = new(this, OldService, service, TargetType);
        ServiceChange?.Invoke(this, EventArgs);

        if (EventArgs.IsCancelled)
        {
            EventArgs.ExecuteActions();
            return null;
        }

        if (EventArgs.ServiceNewValue == null)
        {
            _services.Remove(TargetType);
        }
        else if (TargetType.IsAssignableFrom(EventArgs.ServiceNewValue.GetType()))
        {
            _services[TargetType] = EventArgs.ServiceNewValue;
        }
        else
        {
            throw new InvalidOperationException(
                $"The service of type $\"{EventArgs.ServiceNewValue.GetType().FullName}\" " +
                $"cannot be assigned to the type \"{TargetType.FullName}\"");
        }

        EventArgs.ExecuteActions();
        return OldService;
    }
}