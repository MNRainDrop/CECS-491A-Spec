namespace TeamSpecs.RideAlong.Model;

public interface ILog
{
    DateTimeOffset LogTime { get; set; }
    string LogLevel { get; set; }
    string LogCategory { get; set; }
    string LogContext { get; set; }
    string? UserHash { get; set; }
}
