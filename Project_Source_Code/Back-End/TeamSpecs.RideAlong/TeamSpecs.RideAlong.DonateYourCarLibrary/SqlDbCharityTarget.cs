using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.DonateYourCarLibrary.Interfaces;
using TeamSpecs.RideAlong.Model;


namespace TeamSpecs.RideAlong.DonateYourCarLibrary;

public class SqlDbCharityTarget : ISqlDbCharityTarget
{

    private readonly ISqlServerDAO _dao;

    public SqlDbCharityTarget(ISqlServerDAO dao)
    {
        _dao = dao;
    }

    public IResponse RetrieveCharitiesSql()
    {
        var commandSql = "Select * ";
        var fromSql = "From Charity ";


        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();

            var sqlString = commandSql + fromSql;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Charity Retrieval Sql.";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new CharityModel((string)item[0], (string)item[1], (string)item[2]));
            }
            response.HasError = false;
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Charity Retrieval execution failed.";
        }
        return response;
    }
}
