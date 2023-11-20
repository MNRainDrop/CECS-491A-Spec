namespace TeamSpecs.RideAlong.LoggingLibrary;

public class LogFilter : ILogFilter
{
    public LogFilter()
    {
    }

    public LogFilter(int? logIDLowerBound = null, int? logIDUpperBound = null, DateTime? logDateLowerBound = null, DateTime? logDateUpperBound = null, string? logLevel = null, string? logCategory = null, string? logContext = null, string? createdBy = null)
    {
        this.LogIDLowerBound = logIDLowerBound;
        this.LogIDUpperBound = logIDUpperBound;
        this.LogDateLowerBound = logDateLowerBound;
        this.LogDateUpperBound = logDateUpperBound;
        this.LogLevel = logLevel;
        this.LogCategory = logCategory;
        this.LogContext = logContext;
        this.LogCreatedBy = createdBy;
    }

    public int? LogIDLowerBound { get; set; }
    public int? LogIDUpperBound { get; set; }
    public DateTime? LogDateLowerBound { get; set; }
    public DateTime? LogDateUpperBound { get; set; }
    public string? LogLevel { get; set; }
    public string? LogCategory { get; set; }
    public string? LogContext { get; set; }
    public string? LogCreatedBy { get; set; }
    
}