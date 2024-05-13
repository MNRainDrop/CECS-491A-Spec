using Microsoft.Data.SqlClient;
using System.Data;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration;

public class SqlDbUserTarget : IUserTarget
{
    private readonly ISqlServerDAO _dao;


    public SqlDbUserTarget(ISqlServerDAO dao)
    {
        _dao = dao;
    }

    public IResponse GetUpdatedUserSql(IAccountUserModel userModel)
    {
        if (userModel is null)
        {
            throw new ArgumentNullException(nameof(userModel));
        }
        foreach (var property in typeof(IAccountUserModel).GetProperties())
        {
            if (property.GetValue(userModel) is null)
            {
                throw new ArgumentException($"{nameof(property)} must be valid");
            }
        }

        var sqlCommandString = "";
        var response = new Response();
        sqlCommandString = @"
        SELECT *
        FROM UserDetails
        WHERE UID = {userModel.UserId}";

        var sqlCommand = new SqlCommand
        {
            CommandText = sqlCommandString,
            CommandType = CommandType.Text
        };

        try
        {
            #region Execute SQL
            var daoValue = _dao.ExecuteReadOnly(sqlCommand);
            response.ReturnValue = new List<object>()
            {
                daoValue
            };
            #endregion
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "GetUpdatedUser execute failed";
            return response;
        }
        response.HasError = false;
        return response;
    }

    public IResponse PostUpdatedUserSql(IAccountUserModel userAccount, string address, string name, string phone, string accountType)
    {
        if (userAccount is null)
        {
            throw new ArgumentNullException(nameof(userAccount));
        }
        foreach (var property in typeof(IAccountUserModel).GetProperties())
        {
            if (property.GetValue(userAccount) is null)
            {
                throw new ArgumentException($"{nameof(property)} must be valid");
            }
        }

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();
        try
        {
            string sqlCommand = @"
                UPDATE UserDetails
                SET ";

            List<string> updateStatements = new List<string>();

            HashSet<SqlParameter> parameters = new HashSet<SqlParameter>();

            if (address != null)
            {
                updateStatements.Add("Address = @Address");
                parameters.Add(new SqlParameter("@Address", address));
            }
            if (name != null)
            {
                updateStatements.Add("Name = @Name");
                parameters.Add(new SqlParameter("@Name", name));
            }
            if (phone != null)
            {
                updateStatements.Add("Phone = @Phone");
                parameters.Add(new SqlParameter("@Phone", phone));
            }

            updateStatements.Add("AccountType = @AccountType");
            parameters.Add(new SqlParameter("@AccountType", accountType));

            sqlCommand += string.Join(",", updateStatements);

            sqlCommand += " WHERE UID = @UID";
            parameters.Add(new SqlParameter("@UID", userAccount.UserId));

            sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(sqlCommand, parameters));

            //need to update claims depending on AccountType
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate SQL for updating user";
            return response;

        }

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
            response.ErrorMessage = "Failed to update user";
            return response;
        }

        response.HasError = false;
        return response;

    }
}