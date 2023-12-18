using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
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
        var accountModificationService = new AccountModificationService(new SqlDbUserTarget(_DAO));
        var testUsername = "testemail@gmail.com";
        var testDateTime = DateTime.Now;

        // Act
        response = accountModificationService.ModifyUserProfile(testUsername, testDateTime);

        // Assert
        Assert.False(response.HasError);
        Assert.Null(response.ErrorMessage);
    }

    [Fact]
    public void AccountModificationSerivce_ModifyUserProfile_NullUserNamePassedIn_ArguementExceptionThrown()
    {
        // Arrange
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountModificationService = new AccountModificationService(new SqlDbUserTarget(_DAO));
        string testUsername = null;
        var testDateTime = DateTime.Now;

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountModificationService.ModifyUserProfile(testUsername, testDateTime)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }
    }

    [Fact]
    public void AccountModificationService_ModifyUserProfile_EmptyUserNamePassedIn_ArguementExceptionThrown()
    {
        // Arrange
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountModificationService = new AccountModificationService(new SqlDbUserTarget(_DAO));
        string testUsername = "";
        var testDateTime = DateTime.Now;

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountModificationService.ModifyUserProfile(testUsername, testDateTime)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }
    }

    [Fact]
    public void AccountModificationService_ModifyUserProfile_WhiteSpaceUserNamePassedIn_ArguementExceptionThrown()
    {
        // Arrange
        IResponse response;
        var _DAO = new SqlServerDAO();
        var accountModificationService = new AccountModificationService(new SqlDbUserTarget(_DAO));
        string testUsername = "           ";
        var testDateTime = DateTime.Now;

        // Act and Assert
        try
        {
            Assert.Throws<ArgumentException>(
                () => response = accountModificationService.ModifyUserProfile(testUsername, testDateTime)
            );
        }
        catch
        {
            Assert.Fail("Should throw ArgumentException");
        }
    }

}
