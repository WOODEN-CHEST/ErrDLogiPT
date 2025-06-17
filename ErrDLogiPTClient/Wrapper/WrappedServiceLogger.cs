using ErrDLogiPTClient.Service;
using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrappedServiceLogger : ILogger, IServiceWrapperObject
{
    // Fields.
    public event EventHandler<LoggerLogEventArgs>? LogMessage;


    // Private fields.
    private readonly IGenericServices _services;


    // Constructors.
    public WrappedServiceLogger(IGenericServices services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }


    // Private methods.
    private void OnLogMessageEvent(object? sender, LoggerLogEventArgs args)
    {
        LoggerLogEventArgs WrappedArgs = new(args.Level, args.Message, args.TimeStamp);
        LogMessage?.Invoke(this, WrappedArgs);
        args.Message = WrappedArgs.Message;
        args.TimeStamp = WrappedArgs.TimeStamp;
        args.Level = WrappedArgs.Level;
    }

    private void SubscribeToLogger(ILogger logger)
    {
        logger.LogMessage += OnLogMessageEvent;
    }

    private void UnsubscribeFromLogger(ILogger logger)
    {
        logger.LogMessage -= OnLogMessageEvent;
    }

    private void OnServiceChangeEvent(object? sender, GenericServicesChangeEventArgs args)
    {
        args.AddSuccessAction(() =>
        {
            if (!args.ServiceType.Equals(typeof(ILogger)))
            {
                return;
            }

            if (args.ServiceOldValue != null)
            {
                UnsubscribeFromLogger((ILogger)args.ServiceOldValue);
            }
            if (args.ServiceNewValue != null)
            {
                SubscribeToLogger((ILogger)args.ServiceNewValue);
            }
        });
    }

    private ILogger GetLogger()
    {
        return _services.GetRequired<ILogger>();
    }


    // Inherited methods.
    public string ConvertToLoggedMessage(LogLevel level, DateTime timeStamp, string message)
    {
        return GetLogger().ConvertToLoggedMessage(level, timeStamp, message);
    }

    public void Critical(string message)
    {
        GetLogger().Critical(message);
    }

    public void Error(string message)
    {
        GetLogger().Error(message);
    }

    public void Info(string message)
    {
        GetLogger().Info(message);
    }

    public void Log(LogLevel level, string message)
    {
        GetLogger().Log(level, message);
    }

    public void Warning(string message)
    {
        GetLogger().Warning(message);
    }

    public void InitializeWrapper()
    {
        SubscribeToLogger(GetLogger());
        _services.ServiceChange += OnServiceChangeEvent;
    }

    public void DeinitializeWrapper()
    {
        _services.GetRequired<ILogger>().LogMessage -= OnLogMessageEvent;
        _services.ServiceChange -= OnServiceChangeEvent;
    }

    public void Dispose() { }
}