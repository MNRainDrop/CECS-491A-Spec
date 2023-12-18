using Azure;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration;

namespace TeamSpecs.RideAlong.TestingLibrary;

public class AccountDeletionServiceShoud
{
    [Fact]
    public void AccountDeletionService_DeleteUserAccount_ValidUserNamePassedIn_AccountDeleted_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountDeletionService = new AccountDeletionService(new SqlDbUserTarget(_DAO));
        var testUsername = "testmail@gmail.com";

        // Expected Outcome
        var expectedHasError = false;
        var expectedReturnValue = (object) 1;

        // Undo
        var accountSql = $"INSERT INTO UserAccount (UserName, DateCreated) VALUES ('{testUsername}', GETDATE())";
        var claimsSql = $"INSERT INTO UserClaim (UserID, Claim, ClaimScope) VALUES ((SELECT TOP 1 UserID FROM UserAccount Where UserName = '{testUsername}'), 'isTest', 'True')";
        var profileSql = $"INSERT INTO UserProfile (UserID, DateOfBirth) VALUES ((SELECT TOP 1 UserID FROM UserAccount Where UserName = '{testUsername}'), GETDATE())";
        _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(claimsSql, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(profileSql, null),
        });

        // Act
        timer.Start();
        response = accountDeletionService.DeleteUserAccount(testUsername);
        timer.Stop();

        // Assert
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ReturnValue.Contains(expectedReturnValue));
    }

    [Fact]
    public void AccountDeletionService_DeleteUserAccount_NullUserNamePassedIn_ArgumentExceptionThrown_Pass()
    {
        // Arrange
        var _DAO = new SqlServerDAO();
        IResponse response;
        var accountDeletionService = new AccountDeletionService(new SqlDbUserTarget(_DAO));
        string testUsername = null;

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountDeletionService.DeleteUserAccount(testUsername)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }



    }
}
