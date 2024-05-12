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
    public IResponse CreateUserAccountSql(IAccountUserModel userModel, IDictionary<int, string> userClaims)
    {

        #region Validate Arguments
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
        if (userClaims is null)
        {
            throw new ArgumentNullException(nameof(userClaims));
        }
        foreach (var claim in userClaims)
        {
            if (string.IsNullOrWhiteSpace(claim.Value))
            {
                throw new ArgumentException($"{nameof(claim.Value)}");
            }
        }
        #endregion

        #region Default sql setup
        var commandSql = "INSERT INTO ";
        var tableSql = "";
        var rowsSql = "(";
        var valuesSql = "VALUES (";
        var userNameToID = "(SELECT TOP 1 UserID FROM UserAccount WHERE UserName = ";
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
            foreach (var property in properties)
            {
                rowsSql += property.Name + ",";
                valuesSql += "@" + property.Name + ",";

                var value = property.GetValue(userModel);
                if (value is not null && value.GetType() == typeof(uint))
                {
                    parameters.Add(new SqlParameter("@" + property.Name, Convert.ToInt32(value)));
                }
                else
                {
                    parameters.Add(new SqlParameter("@" + property.Name, value));
                }
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
            rowsSql = "(UserID, ClaimID, ClaimScope) ";
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

            #region Create user profile sql statement
            tableSql = "UserProfile ";
            rowsSql = "(UserID, AlternateUserName, DateCreated) ";
            valuesSql = "VALUES ((SELECT TOP 1 UserID FROM UserAccount WHERE UserName = @UserName), @UserName, GETUTCDATE())";

            parameters = new HashSet<SqlParameter>()
            {

                new SqlParameter("@UserName", userModel.UserName)
            };

            sqlString = commandSql + tableSql + rowsSql + valuesSql;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));

            #endregion

            //Need to ask how valuesSql works
            tableSql = "UserDetails";
            rowsSql = "(UserID, AccountType)";
            valuesSql = "Values((SELECT TOP 1 UserID FROM UserAccount WHERE UserName = @UserName), 'default')";

            parameters = new HashSet<SqlParameter>()
            {

                new SqlParameter("@UserName", userModel.UserName)
            };

            sqlString = commandSql + tableSql + rowsSql + valuesSql;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));

            #region Create user OTP sql statement
            tableSql = "OTP ";
            rowsSql = "(UserID) ";
            valuesSql = "VALUES ((SELECT TOP 1 UserID FROM UserAccount WHERE UserName = @UserName))";

            parameters = new HashSet<SqlParameter>()
            {

                new SqlParameter("@UserName", userModel.UserName)
            };

            sqlString = commandSql + tableSql + rowsSql + valuesSql;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
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
        #region Validate arguments
        if (string.IsNullOrEmpty(userName))
        {
            throw new ArgumentNullException(nameof(userName));
        }
        #endregion

        #region Default sql setup
        var commandSql = "DELETE FROM ";
        var tableSql = "UserAccount ";
        var whereSql = "WHERE UserName = @UserName";
        #endregion


        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        // 
        try
        {
            // create new hash set of SqlParameters
            var parameters = new HashSet<SqlParameter>()
            {
                new SqlParameter("@UserName", userName)
            };

            var sqlString = commandSql + tableSql + whereSql;

            sqlCommands.Add(KeyValuePair.Create<string, HashSet<SqlParameter>?>(sqlString, parameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate sql to delete user";
            return response;
        }

        // DAO Executes the command
        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                (object) daoValue
            };
            response.HasError = false;
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Account Deletion execution failed";
            return response;
        }

        return response;
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
                var propertyValue = property.GetValue(profileModel);

                // Create SQL command for each property
                var sqlCommand = $"UPDATE UserProfile SET {propertyName} = @{propertyName} WHERE UserID = (SELECT TOP 1 UserID FROM UserAccount WHERE UserName = @UserName)";
                var sqlParameters = new HashSet<SqlParameter>
                {
                    new SqlParameter($"@{propertyName}", propertyValue),
                    new SqlParameter("@UserName", userName)
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
            {
                daoValue
            };
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Modification execute failed";
            return response;
        }

        response.HasError = false;
        return response;
    }
    public IResponse EnableUserAccountSql(string userName)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            #region Convert String to SQL

            // Finds claim CanLogin (ClaimID = 1) 
            string sqlCommand = @"
            UPDATE UserClaim
            SET claimScope = 'Yes'
            FROM UserClaim uc
            JOIN UserAccount ua ON uc.UserID = ua.UserID
            WHERE ua.userName = @UserName AND uc.claimID = 1;";

            var parameters = new HashSet<SqlParameter>
        {
            new SqlParameter("@UserName", userName )
        };
            sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(sqlCommand, parameters));
            #endregion 
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate EnableUserAccount Sql";
            return response;
        }

        try
        {
            #region Execute SQL
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                daoValue
            };
            #endregion
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "EnableUserAccount execute failed";
            return response;
        }

        response.HasError = false;
        return response;
    }
    public IResponse DisableUserAccountSql(string userName)
    {
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();
        var response = new Response();

        try
        {
            #region Convert String to SQL

            // Finds claim CanLogin (ClaimID = 1) 
            string sqlCommand = @"
            UPDATE UserClaim
            SET claimScope = 'No'
            FROM UserClaim uc
            JOIN UserAccount ua ON uc.UserID = ua.UserID
            WHERE ua.userName = @UserName AND uc.claimID = 1;";

            var parameters = new HashSet<SqlParameter>
        {
            new SqlParameter("@UserName", userName )
        };
            sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(sqlCommand, parameters));
            #endregion
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate DisableUserAccount Sql";
            return response;
        }

        try
        {
            #region Execute SQL
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            response.ReturnValue = new List<object>()
            {
                daoValue
            };
            #endregion
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "DisableUserAccount execute failed";
            return response;
        }

        response.HasError = false;
        return response;
    }
    public IResponse RecoverUserAccountSql(string userName)
    {
        var sqlCommandString = "";
        var response = new Response();

        #region Convert String to SQL
        sqlCommandString = @"
        SELECT ua.userName, up.alternateUserName
        FROM UserAccount ua
        JOIN UserProfile up ON ua.UserID = up.UserID
        WHERE ua.userName = @UserName;";

        var sqlCommand = new SqlCommand
        {
            CommandText = sqlCommandString,
            CommandType = CommandType.Text
        };
        #endregion


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
            response.ErrorMessage = "RecoverUserAccount execute failed";
            return response;
        }

        response.HasError = false;
        return response;

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