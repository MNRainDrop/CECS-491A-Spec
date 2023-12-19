using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
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
        var testUsername = "Deletetestemail@gmail.com";

        var userAccount = new AccountUserModel(testUsername);

        // Expected Outcome
        var expectedHasError = false;
        var expectedReturnValue = (object) 1;

        // Create
        var accountSql = $"INSERT INTO UserAccount (UserName, Salt) VALUES ('{testUsername}', 0)";
        var claimsSql = $"INSERT INTO UserClaim (UserID, ClaimID, ClaimScope) VALUES ((SELECT TOP 1 UserID FROM UserAccount Where UserName = '{testUsername}'), 1, 'True')";
        var profileSql = $"INSERT INTO UserProfile (UserID, AlternateUserName, DateCreated) VALUES ((SELECT TOP 1 UserID FROM UserAccount Where UserName = '{testUsername}'), '{testUsername}', GETUTCDATE())";
        var otpSql = $"INSERT INTO OTP (UserID) VALUES ((SELECT TOP 1 UserID FROM UserAccount Where UserName = '{testUsername}'))";
        _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(claimsSql, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(profileSql, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(otpSql, null)
        });

        // Act
        timer.Start();
        response = accountDeletionService.DeleteUserAccount(userAccount);
        timer.Stop();

        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
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

        var userAccount = new AccountUserModel(testUsername);

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountDeletionService.DeleteUserAccount(userAccount)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }



    }
}
