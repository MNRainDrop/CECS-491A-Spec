using TeamSpecs.RideAlong.Model;
using System.Diagnostics;
using TeamSpecs.RideAlong.UserAdministration;
using TeamSpecs.RideAlong.DataAccess;

namespace TeamSpecs.RideAlong.TestingLibrary;

public class AccountCreationServiceShould
{
    [Fact]
    public void AccountCreationService_CreateValidUserAccount_ValidUsernameAndDateOfBirthPassedIn_AccountCreated_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountCreationService = new AccountCreationService(new SqlDbUserTarget(_DAO));
        var testUsername = "testemail@gmail.com";
        var testDOB = "1/1/2023";

        // Expected Outcome
        var expectedHasError = false;
        var expectedReturnValue = 1 + typeof(IProfileUserModel).GetProperties().Length;

        // Act
        timer.Start();
        response = accountCreationService.CreateValidUserAccount(testUsername, testDOB);
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
        var accountCreationService = new AccountCreationService(new SqlDbUserTarget(_DAO));
        string testUsername = null;
        string testDOB = "1/1/2023";

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountCreationService.CreateValidUserAccount(testUsername, testDOB)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }
    }
    
    [Fact]
    public void AccountCreationService_CreateValidUserAccount_NullDateOfBirthPassedIn_ArgumentExceptionThrown_Pass()
    {
        // Arrange
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountCreationService = new AccountCreationService(new SqlDbUserTarget(_DAO));
        string testUsername = "testemail@gmail.com";
        string testDOB = null;

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountCreationService.CreateValidUserAccount(testUsername, testDOB)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }
        
    }
}
