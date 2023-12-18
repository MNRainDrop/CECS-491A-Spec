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
        return _logTarget.WriteLog(log);
    }

    public async Task<IResponse> CreateLogAsync(string logLevel, string logCategory, string logContext, string? userHash = null)
    {
        ILog log = new Log(DateTime.UtcNow, logLevel, logCategory, logContext, userHash);
        return await Task.Run(() => _logTarget.WriteLog(log));
    }
}
