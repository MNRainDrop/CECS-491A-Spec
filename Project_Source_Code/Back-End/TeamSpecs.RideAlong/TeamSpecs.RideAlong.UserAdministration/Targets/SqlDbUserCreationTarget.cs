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

        #region Check if user is in DB 
        try
        {
            #region Check if user is fully registered Sql Generation 
            query = @"
                SELECT 'UserAccount' AS Source, UserName AS UserName
                FROM UserAccount
                WHERE UserName = @UserName


                UNION
    
                SELECT 'UserProfile' AS Source, AltUserName AS UserName
                FROM UserProfile
                WHERE AltUserName = @UserName;";
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@UserName", email); ;
            #endregion
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate check for email in DB Sql";
            return response;
        }
        #endregion

        #region DAO Executes the command
        try
        {
            response = _dao.ExecuteReadOnly(cmd);
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Checking for email in SQL DB execution failed";
            return response;
        }
        #endregion

        #region If user does not exist in DB
        if (response.ReturnValue == null)
        {
            response.HasError = false;
            return response;
        }
        #endregion

        // If user exists as AltUserName
        if (response.ReturnValue.Any(r => ((DataRow)r)["Source"].ToString() == "UserProfile"))
        {
            response.HasError = true;
            response.ErrorMessage = "Email exists as alt. UserName";
            return response;
        }

        query = "";
        cmd = new SqlCommand();

        #region User has registered before OR account exists
        try
        {
            #region Check if user is fully registered Sql Generation 
            query = @"
                SELECT 'UserAccount' AS Source, UA.UserName AS UserName
                FROM UserAccount AS UA
                JOIN UserProfile AS UP ON UA.UID = UP.UID
                WHERE UA.UserName = @UserName;";
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@UserName", email); // Assuming email corresponds to username
            #endregion
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate check for email in DB Sql";
            return response;
        }
        #endregion

        #region DAO Executes the command
        try
        {
            response = _dao.ExecuteReadOnly(cmd);
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Checking for email in SQL DB execution failed";
            return response;
        }
        #endregion

        #region If user has not confirmed account 
        if (response.ReturnValue == null)
        {
            // user has not gotten confirmation
            response.HasError = false;
            response.ErrorMessage = "User tables must be updated";
            return response;
        }
        #endregion

        response.HasError = true;
        response.ErrorMessage = "User exists in the Database";
        return response;
    }

    public IResponse CreateUserConfirmation(IAccountUserModel userAccount, string otp)
    {
        IResponse response = new Response();
        DateTime currentUtcTime = DateTime.UtcNow;
        ICollection<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

        #region Try/Catch creating UserAccount/OTP Sql Generation 

        try
        {
            #region Create UserAccount Sql Generation

            // Insert into UserAccount
            string insertUserAccountSql = "INSERT INTO UserAccount (UserName, Salt, UserHash) VALUES (@UserName, @Salt, @UserHash)";
            var userAccountParams = new HashSet<SqlParameter>()
        {
            new SqlParameter("@UserName", userAccount.UserName),
            new SqlParameter("@Salt", userAccount.Salt),
            new SqlParameter("@UserHash", userAccount.UserHash),
        };
            sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(insertUserAccountSql, userAccountParams));

            #endregion

            #region Create OTP Sql Generation

            // Insert into OTP
            string insertOtpSql = "INSERT INTO OTP (UID, PassHash, attempts, firstFailedLogin) " +
                "VALUES ((SELECT UID FROM UserAccount WHERE UserName = @UserName), @PassHash, @Attempts, @FirstFailedLogin)";
            var otpParams = new HashSet<SqlParameter>()
        {
            new SqlParameter("@UserName", userAccount.UserName),
            new SqlParameter("@PassHash", otp),
            new SqlParameter("@Attempts", 0),
            new SqlParameter("@FirstFailedLogin", currentUtcTime)
        };
            sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(insertOtpSql, otpParams));

            #endregion
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate create user confirmation tables.";
            return response;
        }

        #endregion

        #region Try/Catch executing create UserAccount/OTP Sql 

        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            if (daoValue != 0)
            {
                response.ReturnValue = new List<object>();
                response.ReturnValue.Add(daoValue);
            }
            else
            {
                throw new Exception("Rows not inserted");
            }
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not execute create user confirmation tables.";
            return response;
        }

        #endregion

        response.HasError = false;
        return response;
    }

    public IResponse UpdateUserConfirmation(IAccountUserModel userAccount, string otp)
    {
        IResponse response = new Response();
        DateTime currentUtcTime = DateTime.UtcNow;
        ICollection<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommands
        = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

        #region Try/Catch updating UserAccount/OTP Sql Generation 

        try
        {
            #region Update UserAccount Sql Generation 

            // Update UserAccount
            string updateUserAccountSql = "UPDATE UserAccount SET UserHash = @UserHash, Salt = @Salt WHERE UserName = @UserName";
            var userAccountParams = new HashSet<SqlParameter>()
            {
                new SqlParameter("@UserName", userAccount.UserName),
                new SqlParameter("@Salt", userAccount.Salt),
                new SqlParameter("@UserHash", userAccount.UserHash),
            };
            sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(updateUserAccountSql, userAccountParams));

            string updateOtpSql = "UPDATE OTP SET PassHash = @PassHash, attempts = @Attempts, firstFailedLogin = @FirstFailedLogin " +
                "WHERE UID IN (SELECT UID FROM UserAccount WHERE UserName = @UserName)";
            var otpParams = new HashSet<SqlParameter>()
            {
                new SqlParameter("@UserName", userAccount.UserName),
                new SqlParameter("@PassHash", otp),
                new SqlParameter("@Attempts", 0),
                new SqlParameter("@FirstFailedLogin", currentUtcTime)
            };
            sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(updateOtpSql, otpParams));
        #endregion       
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate update user confirmation tables.";
            return response;
        }

        #endregion

        #region Try/Catch executing update UserAccount/ OTP Sql 
        try
        {
            var daoValue = _dao.ExecuteWriteOnly(sqlCommands);
            if (daoValue != 0)
            {
                response.ReturnValue = new List<object>();
                response.ReturnValue.Add(daoValue);
            }
            else
            {
                throw new Exception("Rows not updated");
            }
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not execute update user confirmation tables.";
            return response;

        }
        #endregion

        response.HasError = false;
        return response;
    }

    public IResponse CreateDefaultUser()
    {
        var response = new Response();

        return response;
    }

    public IResponse CreateVendorUser()
    {
        var response = new Response();

        return response;
    }

    public IResponse CreateFleetUser()
    {
        var response = new Response();

        return response;
    }
}


/*
 * -- Table: UserAccount
CREATE TABLE UserAccount (
    UID bigint  NOT NULL IDENTITY(0, 1),
    UserName varchar(50)  NOT NULL,
    Salt int  NOT NULL,
    UserHash varchar(64)  NOT NULL,
    CONSTRAINT UserHash UNIQUE (UserHash),
    CONSTRAINT UserAccount_pk PRIMARY KEY  (UID)
);

-- Table: OTP
CREATE TABLE OTP (
    UID bigint  NOT NULL,
    PassHash varchar(64)  NOT NULL,
    attempts int  NOT NULL,
    firstFailedLogin datetime  NOT NULL,
    CONSTRAINT OTP_pk PRIMARY KEY  (UID)
);
 * 
 */