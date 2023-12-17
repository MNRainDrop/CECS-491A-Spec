namespace TeamSpecs.RideAlong.DataAccess;

using Microsoft.Data.SqlClient;

public interface IWriteOnlyFromDataStore
{
    public int ExecuteWriteOnly(ICollection<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommands);
}
