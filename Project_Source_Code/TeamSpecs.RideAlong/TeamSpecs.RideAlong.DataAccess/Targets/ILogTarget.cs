namespace TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

public interface ILogTarget
{
    public IResponse Write(ILog log);
}