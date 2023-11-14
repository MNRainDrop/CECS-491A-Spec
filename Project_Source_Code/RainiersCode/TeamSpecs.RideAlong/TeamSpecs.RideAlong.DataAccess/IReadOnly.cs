namespace TeamSpecs.RideAlong.DataAccess;

using System.Data.SqlClient;
using TeamSpecs.RideAlong.Model;
public interface IReadOnly
{
    public Response ExectueReadOnly(SqlCommand sql);
}