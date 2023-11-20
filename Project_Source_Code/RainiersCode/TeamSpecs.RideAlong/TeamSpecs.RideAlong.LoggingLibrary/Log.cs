namespace TeamSpecs.RideAlong.LoggingLibrary;

public class Log : ILog
{
    public Log(int? logID, DateTimeOffset logTime, string logLevel, string logCategory, string logContext, string? createdBy)
    {
        this.LogID = logID;
        this.LogTime = logTime;
        this.LogLevel = logLevel;
        this.LogCategory = logCategory;
        this.LogContext = logContext;
        this.LogCreatedBy = createdBy;
    }
    public int? LogID { get; set; }
    public DateTimeOffset LogTime { get; set; }
    public string LogLevel { get; set; }
    public string LogCategory { get; set; }
    public string LogContext { get; set; }
    public string? LogCreatedBy { get; set; }
}