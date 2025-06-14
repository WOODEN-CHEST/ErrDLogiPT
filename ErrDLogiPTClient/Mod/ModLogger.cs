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
    private readonly GenericServices _services;


    // Constructors.
    public ModLogger(GenericServices? services, string modName)
    {
        _modName = modName ?? throw new ArgumentNullException(nameof(modName));
        _services = services ?? throw new ArgumentNullException(nameof(services));
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

    public void Dispose() { }
}