namespace TeamSpecs.RideAlong.SystemObservability;

public interface ISystemObservabilityService:
    IGetAccountCreationAttemptsService,
    IGetLoginAttemptsService,
    IGetTopLongestVisitedViewsService,
    IGetTopMostVisitedViewsService,
    IGetTopRegisteredVehiclesService,
    IGetVehicleCreationAttemptsService,
    IGetLogsService
{
}
