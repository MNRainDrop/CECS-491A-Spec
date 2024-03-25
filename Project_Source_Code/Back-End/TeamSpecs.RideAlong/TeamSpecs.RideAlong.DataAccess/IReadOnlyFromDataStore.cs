namespace TeamSpecs.RideAlong.DataAccess;

using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.Model;

public interface IReadOnlyFromDataStore
{
    public List<object[]> ExecuteReadOnly(ICollection<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommands);

    public IResponse ExecuteReadOnly(SqlCommand sqlCommand);
}