using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.Services.HashService;
using TeamSpecs.RideAlong.UserAdministration;

namespace TeamSpecs.RideAlong.TestingLibrary;

public class AuthenticateUserShould
{
    [Fact]
    public void ReturnTrue()
    {
        bool value = true;
        Assert.True(value);
    }
    [Fact]
    public void RetrieveUserAccountModel()
    {
        //Arrange
        var timer = new Stopwatch();
        var actual = false;
        var expected = true;
        
        IGenericDAO dao = new SqlServerDAO();

        IAuthTarget authTarget = new SQLServerAuthTarget(dao);
        IUserTarget userTarget = new SqlDbUserTarget(dao);
        IPepperTarget pepperTarget = new FilePepperTarget(dao);

        IPepperService pepperService = new PepperService(pepperTarget);
        IHashService hashService = new HashService();
        IAccountCreationService accountCreator = new AccountCreationService(userTarget, pepperService, hashService);
        IAuthenticator authenticator = new AuthService(authTarget);

        var testUserName = "testUser";

        accountCreator.CreateValidUserAccount(testUserName);
        
        
        //Act



        //Assert
        Assert.True(actual == expected);
    }

}
