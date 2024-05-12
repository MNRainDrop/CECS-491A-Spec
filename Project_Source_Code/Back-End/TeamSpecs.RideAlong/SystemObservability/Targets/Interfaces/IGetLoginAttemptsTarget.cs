using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetLoginAttemptsTarget
{
    IResponse GetLoginAttemptsSql(int dateRange);
}
