namespace TeamSpecs.RideAlong.DataAccess;

using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.Model;

public interface IReadOnlyFromDataStore
{
    public IResponse ExecuteReadOnly(SqlCommand sql);
}