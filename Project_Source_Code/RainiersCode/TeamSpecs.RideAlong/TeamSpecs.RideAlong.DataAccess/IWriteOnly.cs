namespace TeamSpecs.RideAlong.DataAccess;

using System.Data.SqlClient;
using TeamSpecs.RideAlong.Model;

public interface IWriteOnly
{
    public IResponse ExecuteWriteOnly(SqlCommand sql);
}
