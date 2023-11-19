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

    public IResponse Write(ILog log)
    {
        // need to protect against sql injection
        // start time and end time I'm not 100% sure of - 11/13

        // removed the "logEndTime parameters because it is not necessary - 11/15
        //Changed to work with log objects
        string sql = $@"INSERT INTO dbo.LoggingTestTable (logStartTime, logLevel, category, description)" +
            "VALUES (@logStartTime, @logLevel, @category, @description)";
        var cmd = new SqlCommand(sql);
        cmd.Parameters.Add(new SqlParameter("@logStartTime", log.logTime));
        cmd.Parameters.Add(new SqlParameter("@logLevel", log.logLevel));
        cmd.Parameters.Add(new SqlParameter("@category", log.logCategory));
        cmd.Parameters.Add(new SqlParameter("@message", log.logContext));
        return _dao.ExectueWriteOnly(cmd);
    }
}