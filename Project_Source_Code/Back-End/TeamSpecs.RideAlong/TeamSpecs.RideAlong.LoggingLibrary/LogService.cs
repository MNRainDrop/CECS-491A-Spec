namespace TeamSpecs.RideAlong.LoggingLibrary;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
public class LogService : ILogService
{
    private readonly ILogTarget _logTarget;
    private readonly IHashService _hashService;

    public LogService(ILogTarget logTarget, IHashService hashService)
    {
        _logTarget = logTarget;
        _hashService = hashService;
    }

    public IResponse CreateLog(string logLevel, string logCategory, string logContext, string? userHash = null)
    {
        //changed to work with log object
        var logHash = _hashService.hashUser(DateTime.UtcNow.ToString() + logLevel + logCategory + logContext + userHash, 0);
        ILog log = new Log(DateTime.UtcNow, logLevel, logCategory, logContext, logHash, userHash);
        return _logTarget.WriteLog(log);
    }

    public async Task<IResponse> CreateLogAsync(string logLevel, string logCategory, string logContext, string? userHash = null)
    {
        var logHash = _hashService.hashUser(DateTime.UtcNow.ToString() + logLevel + logCategory + logContext + userHash, 0);
        ILog log = new Log(DateTime.UtcNow, logLevel, logCategory, logContext, logHash, userHash);
        return await Task.Run(() => _logTarget.WriteLog(log));
    }
}
