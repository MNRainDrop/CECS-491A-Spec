using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration;

namespace TeamSpecs.RideAlong.TestingLibrary;

public class AccountModificationServiceShould
{    
    [Fact]
    public void AccountModificationService_ModifyUserProfile_ValidUserNameAndDateOfBirthPassedIn_ModificationSuccessful_Pass()
    {
        // Arrange
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountModificationService = new AccountModificationService(new SqlDbUserTarget(_DAO), new LogService(new SqlDbLogTarget(), new HashService()), new HashService(), new PepperService(new FilePepperTarget(new JsonFileDAO())));
        var testUsername = "Modifytestemail@gmail.com";
        var testAltUsername = "testAltEmail@gmail.com";
        var testDateTime = DateTime.Now;

        // Create test user
        var accountSql = $"INSERT INTO UserAccount (UserName, Salt) VALUES ('{testUsername}', 0)";
        var claimsSql = $"INSERT INTO UserClaim (UserID, ClaimID, ClaimScope) VALUES ((SELECT TOP 1 UserID FROM UserAccount Where UserName = '{testUsername}'), 1, 'True')";
        var profileSql = $"INSERT INTO UserProfile (UserID, AlternateUserName, DateCreated) VALUES ((SELECT TOP 1 UserID FROM UserAccount Where UserName = '{testUsername}'), '{testUsername}', GETUTCDATE())";
        var otpSql = $"INSERT INTO OTP (UserID) VALUES ((SELECT TOP 1 UserID FROM UserAccount Where UserName = '{testUsername}'))";
        _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(claimsSql, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(profileSql, null),
        });

        // Act
        try
        {
            response = accountModificationService.ModifyUserProfile(testUsername, testDateTime, testAltUsername);
        }
        finally
        {
            // Delete test user
            var sql = $"DELETE FROM UserAccount WHERE UserName = '{testUsername}'";
            _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) });
        }



        // Assert
        Assert.False(response.HasError);
        Assert.Null(response.ErrorMessage);
    }

    [Fact]
    public void AccountModificationSerivce_ModifyUserProfile_NullUserNamePassedIn_ArguementExceptionThrown_Pass()
    {
        // Arrange
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountModificationService = new AccountModificationService(new SqlDbUserTarget(_DAO), new LogService(new SqlDbLogTarget(), new HashService()), new HashService(), new PepperService(new FilePepperTarget(new JsonFileDAO())));
        string testUsername = null;
        var testDateTime = DateTime.Now;

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountModificationService.ModifyUserProfile(testUsername, testDateTime, testUsername)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }
    }

    [Fact]
    public void AccountModificationService_ModifyUserProfile_EmptyUserNamePassedIn_ArguementExceptionThrown_Pass()
    {
        // Arrange
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountModificationService = new AccountModificationService(new SqlDbUserTarget(_DAO), new LogService(new SqlDbLogTarget(), new HashService()), new HashService(), new PepperService(new FilePepperTarget(new JsonFileDAO())));
        string testUsername = "";
        var testDateTime = DateTime.Now;

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountModificationService.ModifyUserProfile(testUsername, testDateTime, testUsername)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }
    }

    [Fact]
    public void AccountModificationService_ModifyUserProfile_WhiteSpaceUserNamePassedIn_ArguementExceptionThrown_Pass()
    {
        // Arrange
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountModificationService = new AccountModificationService(new SqlDbUserTarget(_DAO), new LogService(new SqlDbLogTarget(), new HashService()), new HashService(), new PepperService(new FilePepperTarget(new JsonFileDAO())));
        string testUsername = "           ";
        var testDateTime = DateTime.Now;

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountModificationService.ModifyUserProfile(testUsername, testDateTime, testUsername)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }
    }

}
