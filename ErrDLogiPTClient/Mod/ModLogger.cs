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
    public ModLogger(ILogger? wrappedLogger, string modName)
    {
        _modName = modName ?? throw new ArgumentNullException(nameof(modName));
        _wrappedLogger = wrappedLogger ?? throw new ArgumentNullException(nameof(wrappedLogger));
    }


    // Methods.
    public void Initialize()
    {
        _wrappedLogger.LogMessage += OnLogEvent;
    }


    // Private methods.
    private void OnLogEvent(object? sender, LoggerLogEventArgs args)
    {
        LoggerLogEventArgs LogArgs = new(args.Level, args.Message, args.TimeStamp);
        LogMessage?.Invoke(this, LogArgs);

        args.Message = $"[{_modName}] {LogArgs.Message}";
        args.Level = LogArgs.Level;
        args.TimeStamp = LogArgs.TimeStamp;
    }


    // Inherited methods.
    public string ConvertToLoggedMessage(LogLevel level, DateTime timeStamp, string message)
    {
        return _wrappedLogger.ConvertToLoggedMessage(level, timeStamp, message);
    }

    public void Critical(string message)
    {
        _wrappedLogger.Critical(message);
    }

    public void Error(string message)
    {
        _wrappedLogger.Error(message);
    }

    public void Warning(string message)
    {
        _wrappedLogger.Warning(message);
    }

    public void Info(string message)
    {
        _wrappedLogger.Info(message);
    }

    public void Log(LogLevel level, string message)
    {
        _wrappedLogger.Log(level, message);
    }

    public void Dispose()
    {
        _wrappedLogger.LogMessage -= OnLogEvent;
    }
}