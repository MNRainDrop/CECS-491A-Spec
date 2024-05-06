namespace TeamSpecs.RideAlong.SystemObservability;

public interface ISystemObservabilityTarget:
    IGetAccountCreationAttemptsTarget,
    IGetLoginAttemptsTarget,
    IGetLongestVisitedViewsTarget,
    IGetMostVisitedViewsTarget,
    IGetVehicleCreationAttemptsTarget,
    IGetMostRegisteredVehiclesTarget
{
}
