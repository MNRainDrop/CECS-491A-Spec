using System.Reflection;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public class SystemObservabilityTarget : ISystemObservabilityTarget
{
    public IResponse GetAccountCreationAttemptsSql(int dateRange)
    {
        throw new NotImplementedException();
    }

    public IResponse GetLoginAttemptsSql(int dateRange)
    {
        //Sql will look something like this
        //Select* from Log
        //where LogContext like '%has attempted a login'
        throw new NotImplementedException();
    }

    public IResponse GetLongestVisitedViewsSql(int dateRange)
    {
        throw new NotImplementedException();
    }

    public IResponse GetMostRegisteredVehiclesSql(int dateRange)
    {
        //Sql will look something like this
        //select count(make) as Count, make, model, year
        //from VehicleProfile
        //where dateCreated > DATEADD(m, dateRange, GETUTCDATE())
        //group by Make, Model, Year
        //order by count(make) desc
        throw new NotImplementedException();
    }

    public IResponse GetMostVisitedViewsSql(int dateRange)
    {
        //Sql will look something like this
        //SELECT SUBSTRING(LogContext, 7, len(logcontext) - 6) as Feature, COUNT(*) as Clicks
        //From Log
        //where LogContext like 'Click%'
        //group by SUBSTRING(LogContext, 7, len(logcontext) - 6)
        throw new NotImplementedException();
    }

    public IResponse GetVehicleCreationAttemptsSql(int dateRange)
    {
        //Sql will look something like this
        //select* from log
        //where LogContext like '%vehicle creation.%'
        throw new NotImplementedException();
    }
}
