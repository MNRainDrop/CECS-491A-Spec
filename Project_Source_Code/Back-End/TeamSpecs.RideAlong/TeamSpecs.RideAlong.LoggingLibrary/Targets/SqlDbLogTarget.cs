namespace TeamSpecs.RideAlong.LoggingLibrary;

using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
public class SqlDbLogTarget : ILogTarget
{
    private readonly IGenericDAO _dao;

    public SqlDbLogTarget(IGenericDAO dao)
    {
        _dao = dao;
    }

    public IResponse WriteLog(ILog log)
    {
        #region Default sql setup
        var commandSql = "INSERT INTO ";
        var tableSql = "Log ";
        var rowsSql = "(";
        var valuesSql = "VALUES (";
        #endregion
        
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();

            // convert Log model to sql statement
            var configType = typeof(ILog);
            var properties = configType.GetProperties();

            foreach (var property in properties)
            {
                if(property.GetValue(log) != null)
                {
                    rowsSql += property.Name + ",";
                    valuesSql += "@" + property.Name + ",";

                    parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(log)));
                }
                
            }

            
            rowsSql = rowsSql.Remove(rowsSql.Length - 1, 1);
            valuesSql = valuesSql.Remove(valuesSql.Length - 1, 1);
            rowsSql += ") ";
            valuesSql += ");";

            var sqlString = commandSql + tableSql + rowsSql + valuesSql;

            // Add string and hash set to list that the dao will execute
            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));

        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Log Sql";
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                daoValue
            };
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Log execution failed";
            return response;
        }

        response.HasError = false;
        return response;
    }
}