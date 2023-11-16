namespace TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

public interface ILogTarget
{
    // Not sure if the startTime parameter should be the start time or not - 11/13
    // It should - 11/15
    public Response Write(DateTimeOffset startTime, string logLevel, string category, string? description);
}