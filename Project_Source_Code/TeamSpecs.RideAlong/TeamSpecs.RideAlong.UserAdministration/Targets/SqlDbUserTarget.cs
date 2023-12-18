using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using Microsoft.Data.SqlClient;

namespace TeamSpecs.RideAlong.UserAdministration;

public class SqlDbUserTarget : IUserTarget
{
    private readonly IGenericDAO _dao;


    public SqlDbUserTarget(IGenericDAO dao)
    {
        _dao = dao;
    }
    public IResponse CreateUserAccountSql(IAccountUserModel userModel, IDictionary<string, string> userClaims)
    {

        #region Validate Arguments
        if(userModel is null)
        {
            throw new ArgumentNullException(nameof(userModel));
        }
        foreach(var property in typeof(IAccountUserModel).GetProperties())
        {
            if(property.GetValue(userModel) is null)
            {
                throw new ArgumentException($"{nameof(property)} must be valid");
            }
        }
        if(userClaims is null)
        {
            throw new ArgumentNullException(nameof(userClaims));
        }
        foreach(var claim in userClaims)
        {
            if(string.IsNullOrWhiteSpace(claim.Key))
            {
                throw new ArgumentException($"{nameof(claim.Key)} must be valid");
            }
            if(string.IsNullOrWhiteSpace(claim.Value))
            {
                throw new ArgumentException($"{nameof(claim.Value)}");
            }
        }
        #endregion

        #region Default sql setup
        var commandSql = $"INSERT INTO ";
        var tableSql = "";
        var rowsSql = "(";
        var valuesSql = "VALUES (";
        var userNameToID = "(SELECT UserID FROM UserAccount WHERE UserName = ";
        #endregion

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();


        // Convert Parameters into list of sqlCommands
        try
        {
            #region Convert IAccountUserModel into sql statement
            tableSql = "UserAccount ";
            // Convert UserAccount Model into sql
            var parameters = new HashSet<SqlParameter>();

            // Get properties of UserAccount Model
            var configType = typeof(IAccountUserModel);
            var properties = configType.GetProperties();

            // Modifies the row and values sql to match properties
            // Add SqlParameter to the parameters HashSet
            foreach ( var property in properties )
            {
                rowsSql += property.Name + ",";
                valuesSql += "@" + property.Name + ",";

                parameters.Add(new SqlParameter("@" + property.Name, property.GetValue(userModel)));
            }

            // Truncate the last "," at the end of the sql
            rowsSql = rowsSql.Remove(rowsSql.Length - 1, 1);
            valuesSql = valuesSql.Remove(valuesSql.Length - 1, 1);
            // Add a closing parenthesis at the end of the sql
            rowsSql += ") ";
            valuesSql += ");";
            // Combine the commands into one executable sql string
            var sqlString = commandSql + tableSql + rowsSql + valuesSql;


            // Add string and hash set to list that the dao will execute
            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
            #endregion


            #region Convert IDictionary of claims into sql statement
            tableSql = "UserClaim ";
            rowsSql = "(UserID, Claim, ClaimScope) ";
            valuesSql = "VALUES (" + userNameToID + "@UserName), @Claim, @ClaimScope);";
            // Convert user claims into sql
            foreach (var claim in userClaims)
            {
                parameters = new HashSet<SqlParameter>()
                {

                    new SqlParameter("@UserName", userModel.UserName),
                    new SqlParameter("@Claim", claim.Key),
                    new SqlParameter("@ClaimScope", claim.Value)
                };

                sqlString = commandSql + tableSql + rowsSql + valuesSql;

                sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
            }


            #endregion

        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate AccountCreation Sql";
            return response;
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
            response.ErrorMessage = "AccountCreation execution failed";
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse DeleteUserAccountSql(string userName)
    {
        throw new NotImplementedException();
    }

    public IResponse ModifyUserProfileSql(string userName, IProfileUserModel profileModel)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        #region Validiating Arguements
        if (profileModel is null)
        {
            throw new ArgumentNullException(nameof(profileModel));
        }
        foreach (var property in typeof(IProfileUserModel).GetProperties())
        {
            if (property.GetValue(profileModel) is null)
            {
                throw new ArgumentException($"{nameof(property)} must be valid");
            }
        }
        #endregion

        try
        {
            #region Convert ProfileUserModel object to SQL
            // Get properties of ProfileUserModel object with reflection
            var properties = profileModel.GetType().GetProperties();

            // Iterates thru. each property and generates a SQL command
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(profileModel, null);

                // Create SQL command for each property
                var sqlCommand = $"UPDATE UserProfile SET {propertyName} = @{propertyName} WHERE UserName = @UserName";
                var sqlParameters = new HashSet<SqlParameter>
                {
                    new SqlParameter($"@{propertyName}", propertyValue),
                    new SqlParameter("@UserName", propertyValue)
                };

                sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(sqlCommand, sqlParameters));
            
            }
            #endregion
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate ModifyUserProfile Sql";
            return response;
        }

        // DAO executes command
        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            { daoValue };
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Log Execution failed";
            return response;
        }

        response.HasError = false;
        return response;
    }

    public IResponse RecoverUserAccountSql(string userName)
    {
        throw new NotImplementedException();
    }
}