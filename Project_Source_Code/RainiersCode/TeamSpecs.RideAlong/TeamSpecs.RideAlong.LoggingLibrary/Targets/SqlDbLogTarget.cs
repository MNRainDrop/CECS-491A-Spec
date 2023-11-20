using System.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.LoggingLibrary;

public class SqlDbLogTarget : ILogTarget
{
    private readonly IGenericDAO _dao;

    public SqlDbLogTarget(IGenericDAO dao)
    {
        _dao = dao;
    }

    public IResponse Read(ILogFilter filter)
    {
        string sql = $@"SELECT * FROM dbo.loggingTable ";
        var cmd = new SqlCommand(sql);
        if (filter.LogIDLowerBound is not null)
        {
            cmd.CommandText += @"WHERE logID BETWEEN @lowerBound AND @upperBound";
            cmd.Parameters.Add(new SqlParameter("@lowerBound", filter.LogIDLowerBound));
            cmd.Parameters.Add(new SqlParameter("@upperBound", filter.LogIDUpperBound == null ? filter.LogIDLowerBound : filter.LogIDUpperBound));
        }
        if (filter.LogDateLowerBound is not null)
        {
            cmd.CommandText += @"WHERE logTime BETWEEN @lowerBound AND @upperBound";
            cmd.Parameters.Add(new SqlParameter("@lowerBound", filter.LogDateLowerBound));
            cmd.Parameters.Add(new SqlParameter("@upperBound", filter.LogDateUpperBound == null ? filter.LogDateLowerBound : filter.LogDateUpperBound));
        }
        if (filter.LogLevel is not null)
        {
            cmd.CommandText += @"WHERE logLevel LIKE @logLevel";
            filter.LogLevel = "%" + filter.LogLevel + "%";
            cmd.Parameters.Add(new SqlParameter("@logLevel", filter.LogLevel));
        }
        if (filter.LogCategory is not null)
        {
            cmd.CommandText += @"WHERE logCategory LIKE '%@logCategory%'";
            filter.LogCategory = "%" + filter.LogCategory + "%";
            cmd.Parameters.Add(new SqlParameter("@logCategory", filter.LogCategory));
        }
        if (filter.LogContext is not null)
        {
            cmd.CommandText += @"WHERE logContext LIKE '%@logContext%'";
            filter.LogContext = "%" + filter.LogContext + "%";
            cmd.Parameters.Add(new SqlParameter("@logContext", filter.LogContext));
        }
        return _dao.ExectueReadOnly(cmd);
    }

    public IResponse Write(ILog log)
    {
        // need to protect against sql injection
        // start time and end time I'm not 100% sure of - 11/13

        // removed the "logEndTime parameters because it is not necessary - 11/15
        //Changed to work with log objects
        string sql = $@"INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext, logCreatedBy)" +
            "VALUES (@logTime, @logLevel, @logCategory, @logContext, @logCreatedBy)";
        var cmd = new SqlCommand(sql);
        cmd.Parameters.Add(new SqlParameter("@logTime", log.LogTime));
        cmd.Parameters.Add(new SqlParameter("@logLevel", log.LogLevel));
        cmd.Parameters.Add(new SqlParameter("@logCategory", log.LogCategory));
        cmd.Parameters.Add(new SqlParameter("@logContext", log.LogContext));
        cmd.Parameters.Add(new SqlParameter("@logCreatedBy", log.LogCreatedBy == null ? DBNull.Value : log.LogCreatedBy));
        return _dao.ExectueWriteOnly(cmd);
    }
}