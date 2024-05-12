using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.LoggingLibrary;

namespace TeamSpecs.RideAlong.SystemObservability;

public class SqlDbSystemObservabilityTarget : ISystemObservabilityTarget
{
    private readonly ISqlServerDAO _dao;

    public SqlDbSystemObservabilityTarget(ISqlServerDAO dao)
    {
        _dao = dao;
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetAccountCreationAttemptsSql(int dateRange)
    {
        //Sql will look something like this
        //Select * from Log
        //where AccountCreationAttempt = 1
        #region Default sql setup
        var commandSql = "SELECT LogTime, LogLevel, LogCategory, LogContext ";
        var fromSql = "FROM Log ";
        var whereSql = "WHERE LogTime < DATEADD(m, @dateRange, GETUTCDATE()) and AccountCreationAttempt = 1 ";
        var orderBy = "ORDER BY LogTime DESC ";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@" + nameof(dateRange), dateRange)
            };

            var sqlString = commandSql + fromSql + whereSql + orderBy;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Account Creation Attempts Sql. ";
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
                        logContext: (string)item[3]
                    )
                );
            }
            response.HasError = false;
        }
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = "Vehicle Creation Account Retrieval execution failed. " + ex;
        }
        return response;
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetLoginAttemptsSql(int dateRange)
    {
        //Sql will look something like this
        //Select * from Log
        //where LoginAttempt = 1
        #region Default sql setup
        var commandSql = "SELECT LogTime, LogLevel, LogCategory, LogContext ";
        var fromSql = "FROM Log ";
        var whereSql = "WHERE LogTime < DATEADD(m, @dateRange, GETUTCDATE()) and LoginAttempt = 1 ";
        var orderBy = "ORDER BY LogTime DESC ";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@" + nameof(dateRange), dateRange)
            };

            var sqlString = commandSql + fromSql + whereSql + orderBy;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Creation Attempts Sql. ";
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
                        logContext: (string)item[3]
                    )
                );
            }
            response.HasError = false;
        }
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = "Vehicle Creation Attempts Retrieval execution failed. " + ex;
        }
        return response;
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetLongestVisitedViewsSql(int numOfResults, int dateRange)
    {
        //Sql will look something like this
        //select Top 3 Feature, sum(TimeSpent) as TotalTime from log
        //where TimeSpent is not null
        //group by Feature
        //order by TotalTime desc
        #region Default sql setup
        var commandSql = $"SELECT TOP {numOfResults} FEATURE, sum(TimeSpent) as TotalTime ";
        var fromSql = "FROM Log ";
        var whereSql = "WHERE LogTime < DATEADD(m, @dateRange, GETUTCDATE()) and TimeSpent is not null ";
        var groupby = "GROUP BY Feature ";
        var orderBy = "ORDER BY TotalTime DESC ";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@" + nameof(dateRange), dateRange),
                new SqlParameter("@" + nameof(numOfResults), numOfResults)
            };

            var sqlString = commandSql + fromSql + whereSql + groupby + orderBy;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Top 3 Longest Visited Views Sql. ";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new ViewTimeSpent(
                        timeInSeconds: (int)item[1],
                        feature: (string)item[0]
                    )
                );
            }
            response.HasError = false;
        }
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = "Top 3 Longest Visited Views Retrieval execution failed. " + ex;
        }
        return response;
    }


    /**
     * Works as intended, but is not tested
     */
    public IResponse GetMostRegisteredVehiclesSql(int numOfResults, int dateRange)
    {
        //Sql will look something like this
        //select count(make) as Count, make, model, year
        //from VehicleProfile
        //where dateCreated < DATEADD(m, dateRange, GETUTCDATE())
        //group by Make, Model, Year
        //order by count(make) desc
        #region Default sql setup
        var commandSql = $"SELECT TOP {numOfResults} COUNT(*), Make, Model, Year ";
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
                new SqlParameter("@" + nameof(dateRange), dateRange),
                new SqlParameter("@" + nameof(numOfResults), numOfResults)
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
    
    /**
     * Works as intended, but is not tested
     */
    public IResponse GetMostVisitedViewsSql(int numOfResults, int dateRange)
    {
        //Sql will look something like this
        //select top 3 sum(click) as totalClicks, feature from log
        //where click = 1
        //group by feature
        //order by totalClicks desc
        #region Default sql setup
        var commandSql = $"SELECT TOP {numOfResults} FEATURE, SUM(click) as TotalClicks ";
        var fromSql = "FROM Log ";
        var whereSql = "WHERE LogTime < DATEADD(m, @dateRange, GETUTCDATE()) and Click = 1 ";
        var groupby = "GROUP BY Feature ";
        var orderBy = "ORDER BY TotalClicks DESC ";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@" + nameof(dateRange), dateRange),
                new SqlParameter("@" + nameof(numOfResults), numOfResults)
            };

            var sqlString = commandSql + fromSql + whereSql + groupby + orderBy;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Top 3 Most Visited Views Sql. ";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new ViewClickCount(
                        count: (int)item[1],
                        featureName: (string)item[0]
                    )
                );
            }
            response.HasError = false;
        }
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = "Top 3 Most Visited Views Retrieval execution failed. " + ex;
        }
        return response;
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetVehicleCreationAttemptsSql(int dateRange)
    {
        //Sql will look something like this
        //select * from log
        //where VehicleCreationAttempt = 1
        #region Default sql setup
        var commandSql = "SELECT LogTime, LogLevel, LogCategory, LogContext ";
        var fromSql = "FROM Log ";
        var whereSql = "WHERE LogTime < DATEADD(m, @dateRange, GETUTCDATE()) and VehicleCreationAttempt = 1 ";
        var orderBy = "ORDER BY LogTime DESC ";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@" + nameof(dateRange), dateRange)
            };

            var sqlString = commandSql + fromSql + whereSql + orderBy;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Creation Attempts Sql. ";
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
                        logContext: (string)item[3]
                    )
                );
            }
            response.HasError = false;
        }
        catch (Exception ex)
        {
            response.HasError = true;
            response.ErrorMessage = "Vehicle Creation Attempts Retrieval execution failed. " + ex;
        }
        return response;
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
