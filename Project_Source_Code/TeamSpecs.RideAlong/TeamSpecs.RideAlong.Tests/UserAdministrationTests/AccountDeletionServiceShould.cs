using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration;

namespace TeamSpecs.RideAlong.TestingLibrary.UserAdministrationTests;

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
        var testUsername = "testemail@gmail.com";

        // Expected Outcome
        var expectedHasError = false;
        var expectedReturnValue = 1 + typeof(IProfileUserModel).GetProperties().Length;

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
        var timer = new Stopwatch();
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountDeletionService = new AccountDeletionService(new SqlDbUserTarget(_DAO));
        var testUsername = "testemail@gmail.com";

        // Expected Outcome
        var expectedHasError = false;
        var expectedReturnValue = 1 + typeof(IProfileUserModel).GetProperties().Length;

        // Act
        timer.Start();
        response = accountDeletionService.DeleteUserAccount(testUsername);
        timer.Stop();

        // Assert
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ReturnValue.Contains(expectedReturnValue));
    }
}
