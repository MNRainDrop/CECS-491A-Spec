using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetAllLogsManager
{
    IResponse GetAllLogs(int dateRange);
}
