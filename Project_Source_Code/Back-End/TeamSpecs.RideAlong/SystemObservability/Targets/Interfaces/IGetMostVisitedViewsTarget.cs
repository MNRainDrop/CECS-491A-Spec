using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetMostVisitedViewsTarget
{
    IResponse GetMostVisitedViewsSql(int dateRange);
}

