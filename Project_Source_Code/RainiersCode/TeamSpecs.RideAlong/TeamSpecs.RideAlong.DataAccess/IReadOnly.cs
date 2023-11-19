namespace TeamSpecs.RideAlong.DataAccess;

using System.Data.SqlClient;
using TeamSpecs.RideAlong.Model;

public interface IReadOnly
{
    public IResponse ExectueReadOnly(SqlCommand sql);
}