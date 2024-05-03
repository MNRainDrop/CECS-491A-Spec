using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.ConfigService.ConfigModels;
namespace TeamSpecs.RideAlong.DataAccess;

public class SqlServerDAO : IGenericDAO
{
    private readonly ConnectionStrings _connStrings;
    private readonly IConfigServiceJson _config;

    public SqlServerDAO(IConfigServiceJson config)
    {
        _config = config;
        _connStrings = config.GetConfig().CONNECTION_STRINGS;
    }

    public int ExecuteWriteOnly(ICollection<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommands)
    {
        string _connString = _connStrings.WRITEONLY;

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
        string _connString = _connStrings.READONLY;

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

        string _connString = _connStrings.READONLY;

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
