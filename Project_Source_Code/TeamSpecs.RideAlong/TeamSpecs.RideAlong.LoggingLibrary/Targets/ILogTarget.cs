namespace TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

public interface ILogTarget
{
    public IResponse WriteLog(ILog log);
}