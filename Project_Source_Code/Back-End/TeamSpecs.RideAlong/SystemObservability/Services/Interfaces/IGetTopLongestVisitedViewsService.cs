using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetTopLongestVisitedViewsService
{
    IResponse GetTopLongestVisitedViews(int numOfResults, int timeFrame);
}
