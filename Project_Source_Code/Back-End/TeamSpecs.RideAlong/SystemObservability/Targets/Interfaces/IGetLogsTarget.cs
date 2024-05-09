using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetLogsTarget
{
    IResponse GetLogsSql(int dateRange);
}
