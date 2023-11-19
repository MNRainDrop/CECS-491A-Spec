using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using System.Data.SqlClient;

namespace TeamSpecs.RideAlong.LoggingLibrary;

public class SqlDbLogTarget : ILogTarget
{
    private readonly IGenericDAO _dao;

    public SqlDbLogTarget(IGenericDAO dao)
    {
        _dao = dao;
    }

    public IResponse Write(DateTimeOffset startTime, string logLevel, string category, string? description)
    {
        // need to protect against sql injection
        // start time and end time I'm not 100% sure of - 11/13

        // removed the "logEndTime parameters because it is not necessary - 11/15
        string sql = $@"INSERT INTO dbo.LoggingTestTable (logStartTime, logLevel, category, description)" +
            "VALUES (@logStartTime, @logLevel, @category, @description)";
        var cmd = new SqlCommand(sql);
        cmd.Parameters.Add(new SqlParameter("@logStartTime", startTime));
        cmd.Parameters.Add(new SqlParameter("@logLevel", logLevel));
        cmd.Parameters.Add(new SqlParameter("@category", category));
        cmd.Parameters.Add(new SqlParameter("@message", description));
        return _dao.ExectueWriteOnly(cmd);
    }
}