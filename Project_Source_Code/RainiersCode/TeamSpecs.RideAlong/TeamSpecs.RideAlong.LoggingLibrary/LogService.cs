namespace TeamSpecs.RideAlong.LoggingLibrary;
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

    public IResponse RetrieveLog(ILogFilter logFilter)
    {
        IResponse response = _logTarget.Read(logFilter);
        if (response.ReturnValue is not null)
        {
            var temp = new List<object>();
            foreach (object[] logarray in response.ReturnValue)
            {
                temp.Add(new Log((int)logarray[0], (DateTimeOffset)logarray[1], (string)logarray[2], (string)logarray[3], (string)logarray[4], logarray[5].ToString()));
            }
            response.ReturnValue.Clear();
            response.ReturnValue = temp;
        }
        return response;
    }
}
