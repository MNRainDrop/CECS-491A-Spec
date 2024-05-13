namespace TeamSpecs.RideAlong.Model;

public class Log : ILog
{
    public Log(DateTimeOffset logTime, string logLevel, string logCategory, string logContext, string logHash, string? userHash = null)
    {
        this.LogTime = logTime;
        this.LogLevel = logLevel;
        this.LogCategory = logCategory;
        this.LogContext = logContext;
        this.UserHash = userHash;
        this.LogHash = logHash;
    }


    private int? LogID { get; set; }
    public DateTimeOffset LogTime { get; set; }
    public string LogLevel { get; set; }
    public string LogCategory { get; set; }
    public string LogContext { get; set; }
    public string LogHash { get; set; }
    public string? UserHash { get; set; }
}