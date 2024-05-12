using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetLongestVisitedViewsTarget
{
    IResponse GetLongestVisitedViewsSql(int numOfResults, int dateRange);
}
