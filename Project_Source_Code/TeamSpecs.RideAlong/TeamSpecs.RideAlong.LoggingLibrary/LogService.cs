namespace TeamSpecs.RideAlong.LoggingLibrary;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
public class LogService : ILogService
{
    private readonly ILogTarget _logTarget;

    public LogService(ILogTarget logTarget)
    {
        _logTarget = logTarget;
    }

    public async Task<IResponse> CreateLog(string logLevel, string logCategory, string logContext, string? userHash = null)
    {
        //changed to work with log object
        ILog log = new Log(DateTime.UtcNow, logLevel, logCategory, logContext, userHash);
        IResponse response = await Task.Run(() => _logTarget.WriteLog(log));
        return response;
    }
}
