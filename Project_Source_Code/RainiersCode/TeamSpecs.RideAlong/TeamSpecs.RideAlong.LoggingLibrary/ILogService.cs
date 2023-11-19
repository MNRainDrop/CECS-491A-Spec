namespace TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

public interface ILogService
{
    public IResponse Log(string logLevel, string category, string message);
}