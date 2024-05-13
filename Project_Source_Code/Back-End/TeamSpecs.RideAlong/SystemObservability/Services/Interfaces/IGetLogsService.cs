using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetLogsService
{
    IResponse GetAllLogs(int timeFrame);
}
