namespace TeamSpecs.RideAlong.LoggingLibrary;

using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
    private string createLogHash(string logDetails)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(logDetails));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

    public IResponse CreateLog(string logLevel, string logCategory, string logContext, string? userHash = null)
    {
        DateTime time = DateTime.UtcNow;
        string logDetails = time.ToString() + logLevel + logCategory + logContext + userHash;
        string logHash = createLogHash(logDetails);
        //changed to work with log object
        ILog log = new Log(time, logLevel, logCategory, logContext, logHash, userHash);
        return _logTarget.WriteLog(log);
    }
    /// <summary>
    /// Valid Log Levels:       Info, Debug, Warning, Error
    /// Valid Log Categories:   View, Business, Server, Data, DataStore
    /// Valid Log Context:      Anything in BRD + Create and Leave for System Observability
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="logCategory"></param>
    /// <param name="logContext"></param>
    /// <param name="userHash"></param>
    /// <returns></returns>
    public async Task<bool> CreateLogAsync(string logLevel, string logCategory, string logContext, string? userHash = null, CancellationToken ctoken)
    {
        // Set up log details
        DateTime time = DateTime.UtcNow;
        string logDetails = time.ToString() + logLevel + logCategory + logContext + userHash;
        string logHash = createLogHash(logDetails);
        ILog log = new Log(time, logLevel, logCategory, logContext, logHash, userHash);

        // Set up tcs
        var tcs = new TaskCompletionSource<bool>();
        try
        {
            ctoken.ThrowIfCancellationRequested();
            IResponse logResponse = _logTarget.WriteLog(log);
            tcs.SetResult(!logResponse.HasError);
        }
        catch
        {
            tcs.SetResult(false);
        }

        await tcs.Task;
        return tcs.Task.Result;
    }
}
