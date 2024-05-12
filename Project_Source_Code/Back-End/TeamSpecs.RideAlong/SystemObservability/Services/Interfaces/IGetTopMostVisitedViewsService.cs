using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetTopMostVisitedViewsService
{
    IResponse GetTopMostVisitedViews(int numOfResults, int timeFrame);
}
