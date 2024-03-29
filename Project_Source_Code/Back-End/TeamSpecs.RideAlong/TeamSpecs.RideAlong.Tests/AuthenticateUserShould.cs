﻿using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Bson;
using System.Diagnostics;
using System.Text;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using Xunit.Sdk;

namespace TeamSpecs.RideAlong.TestingLibrary;

// This is a copy of the act + create testing user setupp
/*
        //Act
        #region Generating test user
        var sql = "INSERT INTO UserAccount (UserName, Salt, UserHash)" + $"VALUES ('SecurityTestUser', 123456, 'TestHash')";
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        dao.ExecuteWriteOnly(sqlCommands);
        #endregion
        timer.Start();


        timer.Stop();
        #region Deleting test user
        sql = "DELETE FROM UserAccount WHERE UserName = 'SecurityTestUser'";
        sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        dao.ExecuteWriteOnly(sqlCommands);
        #endregion
*/
#pragma warning disable
public class AuthenticateUserShould
{
    private string GenerateRandomHash()
    {
        string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVQXYZ0123456789";
        int length = 64; //Length of a hash is 64
        StringBuilder sb = new StringBuilder(length);
        Random random = new Random();
        for (int i = 0; i < length; i++)
        {
            int index = random.Next(AllowedChars.Length);
            sb.Append(AllowedChars[index]);
        }
        return sb.ToString();
    }


    [Fact]
    public void Authenticate_A_Username_pass()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var logger = new LogService(logTarget); 
        var authRequest = new AuthNRequest("UserName", "OTP");
        var otp = "OTP";
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        var timer = new Stopwatch();
        var expectedResult = true;
        var actualResult = false;

        //Act
        timer.Start();
        actualResult = authService.Authenticate(authRequest, otp);
        timer.Stop();

        //Assert
        Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void GetUserModel_FromDatabase_Pass()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var logger = new LogService(logTarget);
        var authRequest = new AuthNRequest("UserName", "OTP");
        var otp = "OTP";
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        var timer = new Stopwatch();
        var expectedResult = true;
        var actualResult = false;
        var sql = "INSERT INTO UserAccount (UserName, Salt, UserHash)" + $"VALUES ('SecurityTestUser', 123456, {GenerateRandomHash()})";
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        IResponse response;
        IAuthUserModel authModel;

        //Act
        dao.ExecuteWriteOnly(sqlCommands);
        timer.Start();
        response = authService.GetUserModel("SecurityTestUser");
        if (response.ReturnValue is not null)
        {
            //Check if we got what we ex
            authModel = (AuthUserModel)response.ReturnValue.First();
            AuthUserModel expectedAuthModel = new AuthUserModel(123, "SecurityTestUser", BitConverter.GetBytes(123456), "TestHash");//The UID doesn't matter here
            if (expectedAuthModel.userName.Equals(authModel.userName) & expectedAuthModel.salt.SequenceEqual(authModel.salt) & expectedAuthModel.userHash.Equals(authModel.userHash))
            { actualResult = true; }

            //Check to make sure we are not getting anything unexpected
            authModel = (AuthUserModel)response.ReturnValue.First();
            AuthUserModel unExpectedAuthModel = new AuthUserModel(123, "SecurityTestUser", BitConverter.GetBytes(654321), "TestHash");//The UID doesn't matter here
            if (unExpectedAuthModel.userName.Equals(authModel.userName) & unExpectedAuthModel.salt.SequenceEqual(authModel.salt) & unExpectedAuthModel.userHash.Equals(authModel.userHash))
            { actualResult = false; }

            //Check to make sure we are not getting anything unexpected
            authModel = (AuthUserModel)response.ReturnValue.First();
            AuthUserModel unExpectedAuthModel2 = new AuthUserModel(123, "NotSecurityTestUser", BitConverter.GetBytes(123456), "TestHash");//The UID doesn't matter here
            if (unExpectedAuthModel2.userName.Equals(authModel.userName) & unExpectedAuthModel2.salt.SequenceEqual(authModel.salt) & unExpectedAuthModel2.userHash.Equals(authModel.userHash))
            { actualResult = false; }

            //Check to make sure we are not getting anything unexpected
            authModel = (AuthUserModel)response.ReturnValue.First();
            AuthUserModel unExpectedAuthModel3 = new AuthUserModel(123, "SecurityTestUser", BitConverter.GetBytes(123456), "NotTestHash");//The UID doesn't matter here
            if (unExpectedAuthModel3.userName.Equals(authModel.userName) & unExpectedAuthModel3.salt.SequenceEqual(authModel.salt) & unExpectedAuthModel3.userHash.Equals(authModel.userHash))
            { actualResult = false; }
        }

        timer.Stop();
        sql = "DELETE FROM UserAccount WHERE UserName = 'SecurityTestUser'";
        sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        dao.ExecuteWriteOnly(sqlCommands);
        
        //Assert
        Assert.Equal(expectedResult, actualResult);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }
    [Fact]
    public void Not_GetUserPrincpal_if_No_Such_User_Exists_Fail()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var logger = new LogService(logTarget);
        var authRequest = new AuthNRequest("UserName", "OTP");
        var otp = "OTP";
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        var timer = new Stopwatch();
        var expectedResult = false;
        var actualResult = false;
        IResponse response;
        IAuthUserModel authModel;

        //Act
        timer.Start();
        response = authService.GetUserModel("SecurityTestUser");
        var errorMessage = response.ErrorMessage;
        if (response.ReturnValue is not null)
        {
            //Check if we got what we ex
            authModel = (AuthUserModel)response.ReturnValue.First();
            AuthUserModel expectedAuthModel = new AuthUserModel(123, "SecurityTestUser", BitConverter.GetBytes(123456), GenerateRandomHash());//The UID doesn't matter here
            if (expectedAuthModel.userName.Equals(authModel.userName) & expectedAuthModel.salt.SequenceEqual(authModel.salt) & expectedAuthModel.userHash.Equals(authModel.userHash))
            { actualResult = true; }
        }
        timer.Stop();

        //Assert
        Assert.Equal(expectedResult, actualResult);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }
    [Fact]
    public void getUserPrincipal()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var logger = new LogService(logTarget);
        var authRequest = new AuthNRequest("UserName", "OTP");
        var otp = "OTP";
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        var timer = new Stopwatch();
        var expectedResult = true;
        var actualResult = false;
        IResponse response;

        //Act
        #region Generating test user
        var sql = $"INSERT INTO UserAccount (UserName, Salt, UserHash)" + $"VALUES ('GetUserPrincipalSecurityTestUser', 123456, '{GenerateRandomHash()}');" +
            $"DECLARE @UID BIGINT; SET @UID = SCOPE_IDENTITY();" +
            $"INSERT INTO UserClaim(UID, ClaimID, ClaimScope) VALUES (@UID, 1, 'true')";
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        dao.ExecuteWriteOnly(sqlCommands);
        #endregion

        IAuthUserModel model = (AuthUserModel)(authService.GetUserModel("SecurityTestUser")).ReturnValue.First();
        timer.Start();

        response = authService.GetUserPrincipal(model);
        if (response.HasError != true )
        {
            if (response.ReturnValue is not null)
            {
                if (response.ReturnValue.First() is IAppPrincipal)
                {
                    actualResult = true;
                }
            }
        }


        timer.Stop();
        #region Deleting test user
        sql = "DELETE FROM UserAccount WHERE UserName = 'GetUserPrincipalSecurityTestUser'";
        sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        dao.ExecuteWriteOnly(sqlCommands);
        #endregion

        //Assert
        Assert.True(actualResult ==  expectedResult);
        Assert.True(timer.Elapsed.TotalSeconds < 3);

    }


}
#pragma warning restore