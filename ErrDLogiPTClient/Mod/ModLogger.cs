using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.Mod;

public class ModLogger : ILogger
{
    // Fields.
    public event EventHandler<LoggerLogEventArgs>? LogMessage;


    // Private fields.
    private readonly string _modName;
    private readonly ILogger _wrappedLogger;


    // Constructors.
    public ModLogger(ILogger wrappedLogger, string modName)
    {
        _modName = modName ?? throw new ArgumentNullException(nameof(modName));
        _wrappedLogger = _wrappedLogger ?? throw new ArgumentNullException(nameof(_wrappedLogger));
    }


    // Methods.
    public void Initialize()
    {
        _wrappedLogger.LogMessage += OnLogMessageEvent;
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


    // Inherited methods.
    public string ConvertToLoggedMessage(LogLevel level, DateTime timeStamp, string message)
    {
        return _wrappedLogger.ConvertToLoggedMessage(level, timeStamp, message);
    }

    public void Critical(string message)
    {
        Log(LogLevel.CRITICAL, message);
    }

    public void Error(string message)
    {
        Log(LogLevel.Error, message);
    }

    public void Warning(string message)
    {
        Log(LogLevel.Warning, message);
    }

    public void Info(string message)
    {
        Log(LogLevel.Info, message);
    }

    public void Log(LogLevel level, string message)
    {
        LoggerLogEventArgs LogArgs = new(level, message, DateTime.Now);
        LogMessage?.Invoke(this, LogArgs);

        _wrappedLogger.Log(level, $"[{_modName}] {LogArgs.Message}");
    }

    public void Dispose()
    {
        _wrappedLogger.LogMessage -= OnLogMessageEvent;
    }
}