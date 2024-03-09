namespace TeamSpecs.RideAlong.Model;

public class Log : ILog
{
    public Log(DateTimeOffset logTime, string logLevel, string logCategory, string logContext, string? UserHash = null)
    {
        this.LogTime = logTime;
        this.LogLevel = logLevel;
        this.LogCategory = logCategory;
        this.LogContext = logContext;
        this.UserHash = UserHash;
    }
    private int? LogID { get; set; }
    public DateTimeOffset LogTime { get; set; }
    public string LogLevel { get; set; }
    public string LogCategory { get; set; }
    public string LogContext { get; set; }
    public string? UserHash { get; set; }
}