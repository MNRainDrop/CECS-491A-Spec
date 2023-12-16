namespace TeamSpecs.RideAlong.Model;

public interface ILog
{
    int? LogID { get; set; }
    DateTimeOffset LogTime { get; set; }
    string LogLevel { get; set; }
    string LogCategory { get; set; }
    string LogContext { get; set; }
    string? LogCreatedBy { get; set; }
}
