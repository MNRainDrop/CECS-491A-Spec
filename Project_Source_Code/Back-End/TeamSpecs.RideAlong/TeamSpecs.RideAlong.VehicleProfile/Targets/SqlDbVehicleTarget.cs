using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class SqlDbVehicleTarget : IRetrieveVehiclesTarget, IRetrieveVehicleDetailsTarget
{
    private readonly IGenericDAO _dao;

    public SqlDbVehicleTarget(IGenericDAO dao)
    {
        _dao = dao;
    }

    public IResponse ReadVehicleProfileSql(ICollection<object> searchParameters)
    {
        #region Default sql setup
        var commandSql = "Select * ";
        var fromSql = "From VehicleProfile ";
        var defaultWhereSql = "Where ";
        var whereSql = "";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();

            
            if (searchParameters is not null)
            {
                foreach (var item in searchParameters)
                {
                    KeyValuePair<string, long> searchItem;
                    if (item is KeyValuePair<string, long>)
                    {
                        searchItem = (KeyValuePair<string, long>) item;

                        whereSql += defaultWhereSql + searchItem.Key + " = @" + searchItem.Key + " ";
                        parameters.Add(new SqlParameter("@" + searchItem.Key, searchItem.Value));
                    }
                }
            }
            var sqlString = commandSql + fromSql+ whereSql;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile Retrieval Sql";
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new VehicleProfileModel((string)item[0], (long)item[1], (string)item[2], item[3].ToString(), item[4].ToString(), (int)item[5]));
            }
            response.HasError = false;
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Vehicle Profile Retrieval execution failed";
        }
        return response;
    }

    public IResponse ReadVehicleProfileDetailsSql(ICollection<object> searchParameters)
    {
        #region Default sql setup
        var commandSql = "Select * ";
        var fromSql = "From VehicleDetails ";
        var defaultWhereSql = "Where ";
        var whereSql = "";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>();


            if (searchParameters is not null)
            {
                foreach (var item in searchParameters)
                {
                    KeyValuePair<string, long> searchItem;
                    if (item is KeyValuePair<string, long>)
                    {
                        searchItem = (KeyValuePair<string, long>)item;

                        whereSql += defaultWhereSql + searchItem.Key + " = @" + searchItem.Key + " ";
                        parameters.Add(new SqlParameter("@" + searchItem.Key, searchItem.Value));
                    }
                }
            }
            var sqlString = commandSql + fromSql + whereSql;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile Details Retrieval Sql";
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                response.ReturnValue.Add(new VehicleDetailsModel((string)item[0], (string)item[1], (string)item[2]));
            }
            response.HasError = false;
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Vehicle Profile Retrieval execution failed";
        }
        return response;
    }
}
