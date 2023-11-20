namespace TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

public interface ILogTarget
{
    public IResponse Write(ILog log);
    public IResponse Read(ILogFilter filter);
}