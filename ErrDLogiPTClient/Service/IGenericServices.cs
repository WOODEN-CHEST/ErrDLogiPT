using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Service;

/* Look, this is bad, I agree, hidden dependencies and such, but I couldn't think of anything better. */

/// <summary>
/// An object which stores various services such as input, display, render objects and so on.
/// </summary>
public interface IGenericServices
{
    // Fields.

    /// <summary>
    /// The number of services registered.
    /// </summary>
    int ServiceCount { get; }

    /// <summary>
    /// The registered services, based on their type.
    /// </summary>
    IEnumerable<KeyValuePair<Type, object>> Services { get; }

    /// <summary>
    /// Event which is fired before a service is set.
    /// </summary>
    event EventHandler<GenericServicesChangeEventArgs>? ServiceChange;


    // Methods.

    /// <summary>
    /// Gets a service by its type.
    /// </summary>
    /// <typeparam name="T">The type of the service, usually and interface type.</typeparam>
    /// <returns>The service if one exists, otherwise <c>null</c>.</returns>
    T? Get<T>() where T : class;

    /// <summary>
    /// Gets a service by its type. Throws an exception if no such service is found.
    /// </summary>
    /// <typeparam name="T">The type of the service, usually and interface type.</typeparam>
    /// <returns>The service.</returns>
    /// <exception cref="ServiceException"></exception>
    T GetRequired<T>() where T : class;

    /// <summary>
    /// Sets a service by its type. 
    /// </summary>
    /// <typeparam name="T">The type of the service. It must be set to be an interface or base class instead
    /// of a specific service implementation type. This is because the service, when searched by a get method,
    /// searches based on the provided T type argument.</typeparam>
    /// <param name="service">The service to set.</param>
    /// <returns>The old service which is replaced by the new one passed to this function, or <c>null</c>
    /// if there was no old service.</returns>
    /// <exception cref="ArgumentException"></exception>
    T? Set<T>(T? service) where T : class;
}