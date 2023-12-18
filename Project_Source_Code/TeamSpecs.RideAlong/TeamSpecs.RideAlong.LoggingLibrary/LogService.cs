namespace TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
public class LogService : ILogService
{
    private readonly ILogTarget _logTarget;

    public LogService(ILogTarget logTarget)
    {
        _logTarget = logTarget;
    }

    public IResponse CreateLog(string logLevel, string logCategory, string logContext, string? userHash = null)
    {
        //changed to work with log object
        ILog log = new Log(DateTime.UtcNow, logLevel, logCategory, logContext, userHash);
        IResponse response = _logTarget.WriteLog(log);
        return response;
    }
}
