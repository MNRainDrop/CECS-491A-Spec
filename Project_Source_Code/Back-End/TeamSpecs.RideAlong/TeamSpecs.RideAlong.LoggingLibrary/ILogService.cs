namespace TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

public interface ILogService
{
    IResponse CreateLog(string logLevel, string category, string message, string? userHash);
    Task<IResponse> CreateLogAsync(string logLevel, string category, string message, string? userHash);
}