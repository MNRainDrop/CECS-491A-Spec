using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetAccountCreationAttemptsService
{
    IResponse GetAccountCreationAttempts(int timeFrame);
}
