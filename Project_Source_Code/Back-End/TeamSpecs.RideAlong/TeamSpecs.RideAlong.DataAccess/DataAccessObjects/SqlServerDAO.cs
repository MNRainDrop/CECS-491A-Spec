using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Model.ConfigModels;
namespace TeamSpecs.RideAlong.DataAccess;

public class SqlServerDAO : IGenericDAO
{
    ConnectionStrings _connStrings;

    public SqlServerDAO()
    {
        var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
#pragma warning disable CS8604 // Possible null reference argument.
        var configPath = Path.Combine(directory, "..", "..", "..", "..", "RideAlongConfiguration.json");
        var configuration = new ConfigurationBuilder().AddJsonFile(configPath, optional: false, reloadOnChange: true).Build();
        var section = configuration.GetSection("ConnectionStrings");
        _connStrings = new ConnectionStrings(section["readOnly"], section["writeOnly"], section["admin"]);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    public int ExecuteWriteOnly(ICollection<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommands)
    {
        string _connString = _connStrings.writeOnly;

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
                    Thread.Sleep(5);
                }
                transaction.Commit();
            }
            return rowsAffected;
        }
    }

    public IResponse ExecuteReadOnly(SqlCommand sql)
    {
        string _connString = _connStrings.readOnly;

        var response = new Response()
        {
            ReturnValue = new List<object>()
        };
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
    public List<object[]> ExecuteReadOnly(ICollection<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommands)
    {

        string _connString = _connStrings.readOnly;

        var returnList = new List<object[]>();

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
                            var reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                var arr = new object[reader.FieldCount];

                                reader.GetValues(arr);
                                returnList.Add(arr);
                            }
                            reader.Close();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                transaction.Commit();
            }
            return returnList;
        }
    }
    public IResponse ExecuteReadOnly()
    {
        throw new NotImplementedException();
    }

    public IResponse ExecuteWriteOnly(string value)
    {
        throw new NotImplementedException();
    }


}
