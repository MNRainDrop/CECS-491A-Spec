using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetTopRegisteredVehiclesService
{
    IResponse GetTopRegisteredVehicles(int numOfResults, int timeFrame);
}
