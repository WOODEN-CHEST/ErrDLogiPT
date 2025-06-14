using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient;

/// <summary>
/// Event args for an event fired from a <see cref="GenericServices"/> object when a service changes
/// (added, removed or replaced).
/// <para>If this event is canceled, the service set operation is not executed.</para>
/// </summary>
public class GenericServicesChangeEventArgs : CancellableEventBase
{
    // Fields.

    /// <summary>
    /// The generic services which this event was fired from.
    /// </summary>
    public GenericServices Services { get; private init; }

    /// <summary>
    /// The type of the service.
    /// </summary>
    public Type ServiceType { get; private init; }

    /// <summary>
    /// The service object which currently exists for this type of service, or <c>null</c>
    /// if no service of this type currently exists.
    /// </summary>
    public object? ServiceOldValue { get; private init; }

    /// <summary>
    /// The service which will replace the old service on the success of this event, or <c>null</c>.
    /// if the service is being removed.
    /// <para>This value can be changed to alter which service should the old one be replaced with,
    /// but the new service MUST be either <c>null</c> or a service of the type <see cref="ServiceType"/>.</para>
    /// </summary>
    public object? ServiceNewValue { get; set; }


    // Constructors.
    public GenericServicesChangeEventArgs(GenericServices services, 
        object? serviceOldValue,
        object? serviceNewValue,
        Type serviceType)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        ServiceOldValue = serviceOldValue;
        ServiceNewValue = serviceNewValue;
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
    }
}