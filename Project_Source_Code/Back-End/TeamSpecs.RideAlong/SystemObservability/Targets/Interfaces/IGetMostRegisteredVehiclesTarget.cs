using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetMostRegisteredVehiclesTarget
{
    IResponse GetMostRegisteredVehiclesSql(int dateRange);
}
