namespace TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
public class SqlServerLogService : ILogService
{
    private readonly ILogTarget _logTarget;

    SqlServerLogService(ILogTarget logTarget)
    {
        _logTarget = logTarget;
    }

    public Response Log(string logLevel, string category, string? message)
    {
        var response = new Response();
        // Not sure if the startTime parameter should be the current time of the log being logged or the time the user operation should be 
        _logTarget.Write(DateTime.UtcNow, logLevel, category, message);
        return response;
    }
}
