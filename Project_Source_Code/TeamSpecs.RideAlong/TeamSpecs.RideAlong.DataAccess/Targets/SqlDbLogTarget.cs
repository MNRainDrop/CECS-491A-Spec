namespace TeamSpecs.RideAlong.DataAccess;


using System.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
public class SqlDbLogTarget : ILogTarget
{
    private readonly IGenericDAO _dao;

    public SqlDbLogTarget(IGenericDAO dao)
    {
        _dao = dao;
    }

    public IResponse Write(ILog log)
    {
        var response = WriteHelper(log);
        var newResponse = response;
        while(response.IsSafeToRetry == true && response.RetryAttempts < 3)
        {
            if (newResponse.HasError == true)
            {
                if (newResponse.IsSafeToRetry == true)
                {
                    if (response.RetryAttempts < 3)
                    {
                        response.RetryAttempts++;
                        newResponse = WriteHelper(log);
                    }
                    else
                    {
                        response.IsSafeToRetry = false;
                    }
                }
                else
                {
                    response.IsSafeToRetry = false;
                }
            }
            else
            {
                response.HasError = false;
                response.IsSafeToRetry = false;
            }
        }
        return response;
    }
    public IResponse WriteHelper(ILog log)
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
        cmd.Parameters.Add(new SqlParameter("@logContext", log.LogContext.Length >= 100 ? log.LogContext.Substring(0,100) : log.LogContext));
        cmd.Parameters.Add(new SqlParameter("@logCreatedBy", log.LogCreatedBy == null ? DBNull.Value : log.LogCreatedBy));

        return _dao.ExecuteWriteOnly(cmd);
    }
}