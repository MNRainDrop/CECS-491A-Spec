using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetLoginAttemptsService
{
    IResponse GetLoginAttempts(int timeFrame);
}
