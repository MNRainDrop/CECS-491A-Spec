using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using System.Linq;

namespace TeamSpecs.RideAlong.UserAdministration;

public class SqlDbUserRetrievalTarget : ISqlDbUserRetrievalTarget
{
    private readonly ISqlServerDAO _dao;



    public SqlDbUserRetrievalTarget(ISqlServerDAO dao)
    {
        _dao = dao;
    }

    public IResponse RetrieveAllUserInformation(long UID)
    {
        #region Sql setup
        var commandSql = "Select * ";
        var fromSql = "FROM UserAccount ua ";
        var joinSql = "Inner Join UserProfile ON ua.UID = UserProfile.uid ";
        var joinSql2 = "Inner Join UserDetails ON ua.UID = UserDetails.uid ";
        var whereSql = "WHERE ua.UID = @UID";
        #endregion

        // Convert parameters into sql
        var parameters = new HashSet<SqlParameter>
            {
                new SqlParameter("@UID", UID)
            };


        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            // create new hash set of SqlParameters
            var sqlString = commandSql + fromSql + joinSql + joinSql2 + whereSql;
            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile Retrieval Sql.";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();

            foreach (var item in daoValue)
            {
                //response.ReturnValue.Add(new UserDataRequestModel((string)item[0], (long)item[1], (string)item[2], (string)item[3], (string)item[4], (int)item[5], (DateTime)item[6], (int)item[8], (string)item[9], (int)item[10]));
                response.ReturnValue.Add(new UserDataRequestModel((string)item[1]) { UserHash = (string)item[3], DoB = (DateTime)item[5], AltUserName = (string)item[6], CreationDate = (DateTimeOffset)item[7], Address = (string)item[9], Name = (string)item[10], PhoneNumber = (string)item[11] });

            }
            response.HasError = false;
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "User Account Retrieval execution failed.";
        }
        return response;
    }

    public IResponse RetrieveAllAccountInformation()
    {
        #region Sql setup
        var commandSql = "Select * ";
        var fromSql = "FROM UserAccount ua ";
        var joinSql = "Inner Join UserProfile ON ua.UID = UserProfile.uid ";
        var joinSql2 = "Inner Join UserDetails ON ua.UID = UserDetails.uid ";
        #endregion

        // Convert parameters into sql
        var parameters = new HashSet<SqlParameter>();


        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            // create new hash set of SqlParameters
            var sqlString = commandSql + fromSql + joinSql + joinSql2;
            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate Vehicle Profile Retrieval Sql.";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteReadOnly(sqlCommands);
            response.ReturnValue = new List<object>();
            

            foreach (var item in daoValue)
            {
               
                response.ReturnValue.Add(new UserDataRequestModel((string)item[1]) { UserId = (long)item[0], UserHash = (string)item[3], DoB = (DateTime)item[5], AltUserName = (string)item[6], CreationDate = (DateTimeOffset)item[7], Address = (string)item[9], Name = (string)item[10], PhoneNumber = (string)item[11] });

            }
            response.HasError = false;
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "User Account Retrieval execution failed.";
        }
        return response;
    }
}

