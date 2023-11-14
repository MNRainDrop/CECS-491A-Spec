using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using System.Data.SqlClient;

namespace TeamSpecs.RideAlong.LoggingLibrary.Targets;

public class SqlDbLogTarget : ILogTarget
{
    private readonly IGenericDAO _dao;

    SqlDbLogTarget(IGenericDAO dao)
    {
        _dao = dao;
    }

    public Response Write(DateTimeOffset startTime, string logLevel, string category, string? message)
    {
        // need to protect against sql injection
        // start time and end time I'm not 100% sure of
        string sql = $@"INSERT INTO dbo.LoggingTestTable (logStartTime, logEndTime, logLevel, category, message)" +
            "VALUES (@logStartTime, @logEndTime, @logLevel, @category, @message)";
        var cmd = new SqlCommand(sql);
        cmd.Parameters.Add(new SqlParameter("@logStartTime", startTime));
        cmd.Parameters.Add(new SqlParameter("@logEndTime", DateTime.UtcNow));
        cmd.Parameters.Add(new SqlParameter("@logLevel", logLevel));
        cmd.Parameters.Add(new SqlParameter("@category", category));
        cmd.Parameters.Add(new SqlParameter("@message", message));
        return _dao.ExectueWriteOnly(cmd);
    }
}