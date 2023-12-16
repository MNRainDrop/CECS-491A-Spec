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

    public IResponse CreateLog(string logLevel, string logCategory, string logContext, string? createdBy = null)
    {
        //changed to work with log object
        ILog log = new Log(null, DateTime.UtcNow, logLevel, logCategory, logContext, createdBy);
        IResponse response = _logTarget.Write(log);
        return response;
    }
}
