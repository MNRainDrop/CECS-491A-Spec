namespace TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

public interface ILogService
{
    public IResponse CreateLog(string logLevel, string category, string message, string? createdBy);
    public IResponse RetrieveLog(ILogFilter logFilter);
}