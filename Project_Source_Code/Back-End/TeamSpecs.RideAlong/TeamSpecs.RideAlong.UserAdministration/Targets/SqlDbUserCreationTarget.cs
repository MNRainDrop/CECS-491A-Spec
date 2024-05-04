using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration;

public class SqlDbUserCreationTarget: ISqlDbUserCreationTarget
{
    private readonly IGenericDAO _dao;


    public SqlDbUserCreationTarget(IGenericDAO dao)
    {
        _dao = dao;
    }

    public IResponse CheckDbForEmail(string email)
    {
        #region Variables
        IResponse response = new Response();
        var query = "";
        SqlCommand cmd = new SqlCommand();
        #endregion

        // Convert Parameters into list of sqlCommands
        try
        {
            #region Sql Generation 
            query = "SELECT UID, UserName, Salt, userHash FROM UserAccount WHERE UserName = @UserName";
            cmd.CommandText = query;
            cmd.Parameters.Add(email);

            /* Need the following tables --> UserAccount
             * 
             * Merge UserProfile and Claims table call?
             * 
             */

            #endregion
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate check for email in DB Sql";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(cmd);
            response.ReturnValue = new List<object>()
                {
                    daoValue
                };
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Checking for email in SQL DB execution failed";
            return response;
        }

        response.HasError = false;
        return response;
    }

}