using TeamSpecs.RideAlong.Model;
using System.Data.SqlClient;
using System.Data;

namespace TeamSpecs.RideAlong.DataAccess;

public class SqlServerDAO : IGenericDAO
{
    private string connectionString;
    private string server;
    private string database;
    private string access;

    public SqlServerDAO ()
    {
        connectionString = "";
        server = @"LAPTOP-MARLONE\RIDEALONG";
        database = "RideAlong";
        access = "";
    }
    public IResponse ExectueWriteOnly(SqlCommand sql)
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
                var changedRows = sql.ExecuteNonQuery();

                response.ReturnValue = new List<object>();
                response.ReturnValue.Add(changedRows);
            }
            response.HasError = false;
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
    public IResponse ExectueReadOnly(SqlCommand sql)
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
                    var reader = command.ExecuteReader();

                    response.ReturnValue = new List<object>();
                    while (reader.Read())
                    {
                        var values = new object[reader.FieldCount];
                        reader.GetValues(values);
                        response.ReturnValue.Add(values);
                    }
                }
            }
            response.HasError = false;
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
