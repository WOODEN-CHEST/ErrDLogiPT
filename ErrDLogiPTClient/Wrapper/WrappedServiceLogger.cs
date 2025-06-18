using ErrDLogiPTClient.Service;
using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Wrapper;

public class WrappedServiceLogger : ServiceWrapper<ILogger>,  ILogger
{
    // Fields.
    public event EventHandler<LoggerLogEventArgs>? LogMessage;


    // Constructors.
    public WrappedServiceLogger(IGenericServices services) : base(services) { }


    // Private methods.
    private void OnLogMessageEvent(object? sender, LoggerLogEventArgs args)
    {
        LoggerLogEventArgs WrappedArgs = new(args.Level, args.Message, args.TimeStamp);
        LogMessage?.Invoke(this, WrappedArgs);
        args.Message = WrappedArgs.Message;
        args.TimeStamp = WrappedArgs.TimeStamp;
        args.Level = WrappedArgs.Level;
    }


    // Inherited methods.
    protected override void InitService(ILogger service)
    {
        service.LogMessage += OnLogMessageEvent;
    }

    protected override void DeinitService(ILogger service)
    {
        service.LogMessage -= OnLogMessageEvent;
    }

    public string ConvertToLoggedMessage(LogLevel level, DateTime timeStamp, string message)
    {
        return ServiceObject.ConvertToLoggedMessage(level, timeStamp, message);
    }

    public void Critical(string message)
    {
        ServiceObject.Critical(message);
    }

    public void Error(string message)
    {
        ServiceObject.Error(message);
    }

    public void Info(string message)
    {
        ServiceObject.Info(message);
    }

    public void Log(LogLevel level, string message)
    {
        ServiceObject.Log(level, message);
    }

    public void Warning(string message)
    {
        ServiceObject.Warning(message);
    }

    public void Dispose() { }
}