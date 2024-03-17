using TeamSpecs.RideAlong.Model;
using System.Diagnostics;
using TeamSpecs.RideAlong.UserAdministration;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Services;
using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.Services.HashService;
using Microsoft.IdentityModel.Tokens;

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
        var accountCreationService = new AccountCreationService(new SqlDbUserTarget(_DAO), new PepperService(new FilePepperTarget(new JsonFileDAO())), new HashService());
        var testUsername = "Fakeuseraccount@.com";
        var testAccountType = "Default User";
        var testDateOfBirth = new DateTime(2000, 1, 1);

        // Expected Outcome
        var expectedHasError = false;

        // Act
        
        
            timer.Start();
            response = accountCreationService.CreateValidUserAccount(testUsername, testDateOfBirth, testAccountType);
            timer.Stop();
        
        
        
        
            // Undo
            var sql = $"DELETE FROM UserAccount WHERE UserName = '{testUsername}'";
            _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) });
        
        


        // Assert
        //Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(!response.ReturnValue.IsNullOrEmpty());

        
    }


    [Fact]
    public void AccountCreationService_CreateValidUserAccount_NullUsernamePassedIn_ArgumentExceptionThrown_Pass()
    {
        // Arrange
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountCreationService = new AccountCreationService(new SqlDbUserTarget(_DAO), new PepperService(new FilePepperTarget(new JsonFileDAO())), new HashService());
        string testUsername = null;
        var testAccountType = "Default";
        var testDateOfBirth = new DateTime(2000, 1, 1);

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountCreationService.CreateValidUserAccount(testUsername, testDateOfBirth, testAccountType)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }
    }
}
