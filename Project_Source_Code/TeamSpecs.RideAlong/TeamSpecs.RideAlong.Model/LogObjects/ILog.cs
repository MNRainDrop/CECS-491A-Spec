namespace TeamSpecs.RideAlong.Model;

public interface ILog
{
    public int? LogID { get; set; }
    public DateTimeOffset LogTime { get; set; }
    public string LogLevel { get; set; }
    public string LogCategory { get; set; }
    public string LogContext { get; set; }
    public string? LogCreatedBy { get; set; }
}
