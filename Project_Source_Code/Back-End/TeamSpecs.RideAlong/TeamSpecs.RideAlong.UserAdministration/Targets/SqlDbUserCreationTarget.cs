using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.DataAccess;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Reflection;
using System;

namespace TeamSpecs.RideAlong.UserAdministration;

public class SqlDbUserCreationTarget : ISqlDbUserCreationTarget
{
    private readonly ISqlServerDAO _dao;


    public SqlDbUserCreationTarget(ISqlServerDAO dao)
    {
        _dao = dao;
    }

    public IResponse CheckDbForEmail(string email)
    {
        #region Variables
        IResponse response = new Response();
        // If user exists on either UserAccount or UserProfile, the table name will be listed and number 1 [UserAccount, 1]
        string query = @"
            SELECT 'UserAccount' AS Source, UserName AS UserName
            FROM UserAccount
            WHERE UserName = @UserName


            UNION
    
            SELECT 'UserProfile' AS Source, AltUserName AS UserName
            FROM UserProfile
            WHERE AltUserName = @UserName;"; 
        SqlCommand cmd = new SqlCommand();
        #endregion

        #region Check if user is in DB 
        try
        {
            #region Check if user is fully registered Sql Generation 
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@UserName", email);
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
        if (response.ReturnValue is not null && response.ReturnValue.Count == 0)
        {
            response.HasError = false;
            return response;
        }
        #endregion


        #region If user has same altUserName as email arguement
        if (response.ReturnValue is not null && response.ReturnValue.ToList()[0] is object[] array)
        {
            var check = array[0].ToString() == "UserProfile";

            if (response.ReturnValue.Count() == 2)
            {
                check = true;
            }

            // If user exists as AltUserName
            if (check )
            {

                response.HasError = true;
                response.ErrorMessage = "Email exists as alt. UserName";
                return response;
            }
        }
        #endregion


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
        if (response.ReturnValue is not null && response.ReturnValue.Count() == 0 )
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
            new SqlParameter("@Salt", (int)userAccount.Salt),
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
            // Need to specify DB type, or else entering 0 will be intialized to null --> see stack overflow link at bottom
            new SqlParameter("@Attempts", (object) 0),
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
                new SqlParameter("@Salt", (int)userAccount.Salt),
                new SqlParameter("@UserHash", userAccount.UserHash),
            };
            sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(updateUserAccountSql, userAccountParams));

            string updateOtpSql = "UPDATE OTP SET PassHash = @PassHash, attempts = @Attempts, firstFailedLogin = @FirstFailedLogin " +
                "WHERE UID IN (SELECT UID FROM UserAccount WHERE UserName = @UserName)";
            var otpParams = new HashSet<SqlParameter>()
            {
                new SqlParameter("@UserName", userAccount.UserName),
                new SqlParameter("@PassHash", otp),
                new SqlParameter("@Attempts", (object)0),
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

    public IResponse CreateUserProfile(string email, IProfileUserModel profile)
    {
        IResponse response = new Response();
        ICollection<KeyValuePair<string, HashSet<SqlParameter>?>> sqlCommands
        = new List<KeyValuePair<string, HashSet<SqlParameter>?>>();

        #region CreateUserProfile Sql generation 
        try
        {
            string query = $@"
                INSERT INTO UserProfile (UID, DateOfBirth, AltUserName, DateCreated)
                VALUES ((SELECT UID FROM UserAccount Where UserName = @UserName), @DateOfBirth, @AlternateUserName, @DateCreated)
            ";
            var  profileParameters = new HashSet<SqlParameter>
            {
                new SqlParameter("@UserName", email),
                new SqlParameter("@DateOfBirth", profile.DateOfBirth) ,
                new SqlParameter("@AlternateUserName", profile.AlternateUserName),
                new SqlParameter("@DateCreated", profile.DateCreated)
            };
            sqlCommands.Add(new KeyValuePair<string, HashSet<SqlParameter>?>(query, profileParameters));
        }
        catch
        {
            response.HasError = true;
            response.ErrorMessage = "Could not generate create userProfile tables.";
            return response;
        }
        #endregion

        #region CreateUserProfile Sql Exection

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
            response.ErrorMessage = "Could not execute create userProfile tables.";
            return response;

        }

        #endregion

        response.HasError = false;
        return response;
    }

}