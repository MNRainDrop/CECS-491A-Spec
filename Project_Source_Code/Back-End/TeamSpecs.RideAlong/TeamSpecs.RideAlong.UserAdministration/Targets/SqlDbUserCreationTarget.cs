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
            //query = "SELECT UA.*, UP.*\r\nFROM UserAccount AS UA\r\nJOIN UserProfile AS UP ON UA.UID = UP.UID\r\nWHERE UA.UserName = @UserName;";
            query = "SELECT * FROM UserAccount WHERE UserName = @UserName";
            cmd.CommandText = query;
            cmd.Parameters.Add(email);
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

        query = "";
        cmd = new SqlCommand();

        #region User has registered before OR account exists
        try
        {
            #region Check if user Sql Generation 
            query = "SELECT UA.*, UP.*\r\nFROM UserAccount AS UA\r\nJOIN UserProfile AS UP ON UA.UID = UP.UID\r\nWHERE UA.UserName = @UserName;";
            cmd.CommandText = query;
            cmd.Parameters.Add(email);
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

    public IResponse CreateUserConfirmation()
    {
        IResponse response = new Response();



        return response;
    }

    public IResponse UpdateUserConfirmation()
    {
        IResponse response = new Response();

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