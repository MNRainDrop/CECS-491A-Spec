using TeamSpecs.RideAlong.Model;
using System.Diagnostics;
using TeamSpecs.RideAlong.UserAdministration;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.TestingLibrary;

public class AccountCreationServiceShould
{
    [Fact]
    public void AccountCreationService_CreateValidUserAccount_ValidUsernamePassedIn_AccountCreated_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountCreationService = new AccountCreationService(new SqlDbUserTarget(_DAO), new PepperService(), new RandomService());
        var testUsername = "testemail@gmail.com";

        // Expected Outcome
        var expectedHasError = false;
        var expectedReturnValue = 1 + typeof(IProfileUserModel).GetProperties().Length;

        // Act
        timer.Start();
        response = accountCreationService.CreateValidUserAccount(testUsername);
        timer.Stop();
        
        // Assert
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ReturnValue.Contains(expectedReturnValue));
    }

    [Fact]
    public void AccountCreationService_CreateValidUserAccount_NullUsernamePassedIn_ArgumentExceptionThrown_Pass()
    {
        // Arrange
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountCreationService = new AccountCreationService(new SqlDbUserTarget(_DAO), new PepperService(), new RandomService());
        string testUsername = null;

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountCreationService.CreateValidUserAccount(testUsername)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }
    }
}
