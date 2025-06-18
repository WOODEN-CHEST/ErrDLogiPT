using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Service;

public abstract class DefaultGenericServices : IGenericServices
{
    // Fields.
    public int ServiceCount => _services.Count;
    public IEnumerable<KeyValuePair<Type, object>> Services => _services;
    public event EventHandler<GenericServicesChangeEventArgs>? ServiceChange;


    // Private fields.
    private readonly Dictionary<Type, object> _services = new();



    // Protected methods.
    protected virtual bool IsSetServiceValid(object? service)
    {
        return true;
    }


    // Inherited methods.
    public virtual T? Get<T>() where T : class
    {
        Type TargetType = typeof(T);
        if (_services.TryGetValue(TargetType, out object? Service) && (Service is T CastService))
        {
            return CastService;
        }
        return null;
    }


    public virtual T GetRequired<T>() where T : class
    {
        T? Service = Get<T>();
        return Service == null ? throw new ServiceException($"Missing required service of type {typeof(T).FullName}") : Service;
    }


    public virtual T? Set<T>(T? service) where T : class
    {
        Type TargetType = typeof(T);
        T? OldService = Get<T>();
        if (OldService == service)
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

        if (!IsSetServiceValid(EventArgs.ServiceNewValue))
        {
            throw new ArgumentException($"Invalid service: {EventArgs.ServiceNewValue?.GetType().FullName ?? "null"}");
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
            throw new ArgumentException(
                $"The service of type $\"{EventArgs.ServiceNewValue.GetType().FullName}\" " +
                $"cannot be assigned to the type \"{TargetType.FullName}\"");
        }

        EventArgs.ExecuteActions();
        return OldService;
    }
}