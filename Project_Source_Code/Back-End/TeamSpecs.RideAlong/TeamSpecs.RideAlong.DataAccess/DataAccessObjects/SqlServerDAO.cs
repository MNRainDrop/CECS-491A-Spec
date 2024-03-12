using TeamSpecs.RideAlong.Model;
using Microsoft.Data.SqlClient;

namespace TeamSpecs.RideAlong.DataAccess;

public class SqlServerDAO : ISqlServerDAO
{
    private string _connString;
    private readonly string _server;
    private readonly string _database;
    private string _access;

    public SqlServerDAO ()
    {
        _connString = "";
        _server = @"localhost";
        _database = "RideAlongTest";
        _access = "";
    }
    public int ExecuteWriteOnly(ICollection<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommands)
    {
        _access = "User Id=RideAlongWrite;Password=writeme;TrustServerCertificate=True;";
        _connString = $"Server={_server};Database={_database};{_access}";
        var rowsAffected = 0;

        using (var conn = new SqlConnection(_connString))
        {
            conn.Open();
            using (var transaction = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                foreach (var sqlCommand in sqlCommands)
                {
                    using (var command = new SqlCommand(sqlCommand.Key, conn, transaction))
                    {
                        command.CommandType = System.Data.CommandType.Text;

                        if (sqlCommand.Value != null)
                        {
                            foreach (var parameter in sqlCommand.Value)
                            {
                                command.Parameters.Add(parameter);
                            }
                        }

                        try
                        {
                            rowsAffected += command.ExecuteNonQuery();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                transaction.Commit();
                return rowsAffected;
            }
        }
    }

    public IResponse ExecuteReadOnly(SqlCommand sql)
    {
        _access = "User Id=RideAlongRead;Password=readme;TrustServerCertificate=True";
        _connString = $"Server={_server};Database={_database};{_access}";

        var response = new Response();
        try
        {
            
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();

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
            response.IsSafeToRetry = false;
        }
        return response;
    }

}
