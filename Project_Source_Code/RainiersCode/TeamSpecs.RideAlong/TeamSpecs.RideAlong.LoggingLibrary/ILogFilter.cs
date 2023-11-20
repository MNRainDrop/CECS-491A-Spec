namespace TeamSpecs.RideAlong.LoggingLibrary;

public interface ILogFilter
{
    public int? LogIDLowerBound { get; set; }
    public int? LogIDUpperBound { get; set; }
    public DateTime? LogDateLowerBound { get; set; }
    public DateTime? LogDateUpperBound { get; set; }
    public string? LogLevel { get; set; }
    public string? LogCategory { get; set; }
    public string? LogContext { get; set; }
    public string? LogCreatedBy { get; set; }
}
