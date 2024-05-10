using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.LoggingLibrary;

namespace TeamSpecs.RideAlong.SystemObservability;

public class SqlDbSystemObservabilityTarget : ISystemObservabilityTarget
{
    private readonly IGenericDAO _dao;

    public SqlDbSystemObservabilityTarget(IGenericDAO dao)
    {
        _dao = dao;
    }

    public IResponse GetAccountCreationAttemptsSql(int dateRange)
    {
        throw new NotImplementedException();
    }

    public IResponse GetLoginAttemptsSql(int dateRange)
    {
        //Sql will look something like this
        //Select * from Log
        //where LogContext like '%has attempted a login'
        throw new NotImplementedException();
    }

    public IResponse GetLongestVisitedViewsSql(int dateRange)
    {
        throw new NotImplementedException();
    }


    /**
     * Works as intended, but is not tested
     */
    public IResponse GetMostRegisteredVehiclesSql(int dateRange)
    {
        //Sql will look something like this
        //select count(make) as Count, make, model, year
        //from VehicleProfile
        //where dateCreated < DATEADD(m, dateRange, GETUTCDATE())
        //group by Make, Model, Year
        //order by count(make) desc
        #region Default sql setup
        var commandSql = "SELECT TOP 3 COUNT(*), Make, Model, Year ";
        var fromSql = "FROM VehicleProfile ";
        var whereSql = "WHERE DateCreated < DATEADD(m, @dateRange, GETUTCDATE()) ";
        var groupby = "GROUP BY Make, Model, Year ";
        var orderBy = "ORDER BY COUNT(*) DESC ";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@" + nameof(dateRange), dateRange)
            };

            var sqlString = commandSql + fromSql + whereSql + groupby + orderBy;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Log Retrieval Sql. ";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new VehicleMMYCount(
                        count: (int)item[0],
                        make: (string)item[1],
                        model: (string)item[2],
                        year: (int)item[3]
                    )
                );
            }
            response.HasError = false;
        }
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = "Log Retrieval execution failed. " + ex;
        }
        return response;
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
        //select * from log
        //where LogContext like '%vehicle creation.%'
        throw new NotImplementedException();
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetLogsSql(int dateRange)
    {
        //sql will look something like this
        // select * from log
        // where logTime < DATEADD(m, dateRange, GETUTCDATE())
        #region Default sql setup
        var commandSql = "SELECT LogTime, LogLevel, LogCategory, LogContext ";
        var fromSql = "FROM Log ";
        var whereSql = "WHERE LogTime < DATEADD(m, @dateRange, GETUTCDATE())";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@" + nameof(dateRange), dateRange)
            };

            var sqlString = commandSql + fromSql + whereSql;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Log Retrieval Sql. ";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new Log(
                    logTime: (DateTimeOffset)item[0],
                    logLevel: (string)item[1],
                    logCategory: (string)item[2],
                    logContext: (string)item[3])
                );
            }
            response.HasError = false;
        }
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = "Log Retrieval execution failed. " + ex;
        }
        return response;
    }
}
