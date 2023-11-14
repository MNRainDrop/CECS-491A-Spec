using TeamSpecs.RideAlong.Model;
using System.Data.SqlClient;

namespace TeamSpecs.RideAlong.DataAccess;

public class SqlServerDAO : IGenericDAO
{
    private string connectionString = "";
    private readonly string server = @"LAPTOP-MARLONE\RIDEALONG";
    private readonly string database = "RideAlong";
    private string access = "";

    public Response ExectueReadOnly(SqlCommand sql)
    {
        access = "User Id=RideAlongRead;Password=readme;";

        var response = new Response();
        try
        {
            connectionString = $"Server={server};Database={database};{access}";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Protect against sql injection

                sql.Connection = connection;
                using (var command = sql)
                {
                    using (var reader = command.ExecuteReader())
                    {
                        response.ReturnValue = (ICollection<object>)reader;
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            // If connection.Open() or command.ExecuteNonQuery() throws SqlException
            // Executed command against a locked row
            // Timeout during an operation
            response.HasError = true;
            response.ErrorMessage = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            // If connection.Open() or command.ExecuteNonQuery() throws InvalidOperationException
            // SqlConnection could have closed or been dropped during operation
            response.HasError = true;
            response.ErrorMessage = ex.Message;
        }
        return response;
    }

    public Response ExectueWriteOnly(SqlCommand sql)
    {
        access = "User Id=RideAlongWrite;Password=writeme;";

        var response = new Response();
        try
        {
            connectionString = $"Server={server};Database={database};{access}";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                sql.Connection = connection;
                using (var command = sql)
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SqlException ex)
        {
            // If connection.Open() or command.ExecuteNonQuery() throws SqlException
            // Executed command against a locked row
            // Timeout during an operation
            response.HasError = true;
            response.ErrorMessage = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            // If connection.Open() or command.ExecuteNonQuery() throws InvalidOperationException
            // SqlConnection could have closed or been dropped during operation
            response.HasError = true;
            response.ErrorMessage = ex.Message;
        }
        return response;
    }
}
