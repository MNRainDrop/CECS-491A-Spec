using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetVehicleCreationAttemptsService
{
    IResponse GetVehicleCreationAttempts(int timeFrame);
}
