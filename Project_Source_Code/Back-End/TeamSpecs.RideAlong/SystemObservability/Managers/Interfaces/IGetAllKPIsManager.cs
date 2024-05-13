using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public interface IGetAllKPIsManager
{
    IResponse GetALlKPIs(int dateRange);
}
