using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetAccountCreationAttemptsTarget
{
    IResponse GetAccountCreationAttemptsSql(int dateRange);
}
