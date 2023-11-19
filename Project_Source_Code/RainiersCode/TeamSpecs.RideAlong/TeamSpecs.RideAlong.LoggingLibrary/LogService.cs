namespace TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
public class LogService : ILogService
{
    private readonly ILogTarget _logTarget;

    public LogService(ILogTarget logTarget)
    {
        _logTarget = logTarget;
    }

    public IResponse Log(string logLevel, string logCategory, string? logMessage)
    {
        // Not sure if the startTime parameter should be the current time of the log being logged or the time the user operation should be 

        //changed to work with log object
        ILog log = new Log(null, DateTime.UtcNow, logLevel, logCategory, logMessage);
        response = _logTarget.Write(log);
        return response;
    }
}
