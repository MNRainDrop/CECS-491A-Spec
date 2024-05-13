using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetVehicleCreationAttemptsTarget
{
    IResponse GetVehicleCreationAttemptsSql(int dateRange);
}
