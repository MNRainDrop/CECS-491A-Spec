namespace TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

public interface ILogService
{
    IResponse CreateLog(string logLevel, string category, string message, string? userHash);
    /// <summary>
    /// Writes async Log to DB. Valid Log Levels: (Info, Debug, Warning, Error). Valid Categories: (View, Business, Server, Data, DataStore)
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="logCategory"></param>
    /// <param name="logContext"></param>
    /// <param name="userHash"></param>
    /// <returns>Task with a bool indicating success or failure</returns>
    Task<bool> CreateLogAsync(string logLevel, string category, string message, string? userHash, CancellationToken ctoken = default);
}