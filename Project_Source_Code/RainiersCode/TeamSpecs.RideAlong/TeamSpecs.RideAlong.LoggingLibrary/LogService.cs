namespace TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
public class LogService : ILogService
{
    private readonly ILogTarget _logTarget;

    LogService(ILogTarget logTarget)
    {
        _logTarget = logTarget;
    }

    public Response Log(string logLevel, string logCategory, string? logMessage)
    {
        var response = new Response();
        // Not sure if the startTime parameter should be the current time of the log being logged or the time the user operation should be 
        response = _logTarget.Write(DateTime.UtcNow, logLevel, logCategory, logMessage);
        return response;
    }
}
