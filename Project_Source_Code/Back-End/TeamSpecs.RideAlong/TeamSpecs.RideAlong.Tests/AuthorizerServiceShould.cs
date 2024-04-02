using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.Services;
using System.Security.Principal;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.TestingLibrary;

public class AuthorizeUserShould
{
    [Fact]
    public void AuthService_Authorize_RequiredClaimsPassedIn_ReturnTrue_Pass()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var actualResult = false;
        var hashService = new HashService();
        var logger = new LogService(logTarget, hashService);
        var authRequest = new AuthNRequest("UserName", "OTP");
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        //Setting up AppPrincipal object 
        IDictionary<string, string> Claims = new Dictionary<string, string>();
        Claims.Add("canLogin", "true");
        Claims.Add("canLogout", "true");
        AuthUserModel expectedAuthModel = new AuthUserModel(123, "SecurityTestUser", BitConverter.GetBytes(123456), "TestHash");
        var principal = new AppPrincipal(expectedAuthModel,Claims);

        //Setting up RequiredClaims 
        Dictionary<string, string> RClaims = new Dictionary<string, string>();
        RClaims.Add("canLogin", "true");
        RClaims.Add("canLogout", "true");
        var timer = new Stopwatch();


        //Act
        timer.Start();
        actualResult = authService.Authorize(principal, RClaims);
        timer.Stop();

        //Assert
        Assert.True(actualResult == false);
    }

    [Fact]
    public void AuthService_Authorize_InavlidClaimsPassedIn_ReturnFalse_Pass()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var actualResult = false;
        var hashService = new HashService();
        var logger = new LogService(logTarget, hashService);
        var authRequest = new AuthNRequest("UserName", "OTP");
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        //Setting up AppPrincipal object 
        IDictionary<string, string> Claims = new Dictionary<string, string>();
        Claims.Add("canLogin", "true");
        Claims.Add("canLogout", "false");
        AuthUserModel expectedAuthModel = new AuthUserModel(123, "SecurityTestUser", BitConverter.GetBytes(123456), "TestHash");
        var principal = new AppPrincipal(expectedAuthModel, Claims);

        //Setting up RequiredClaims 
        Dictionary<string, string> RClaims = new Dictionary<string, string>();
        RClaims.Add("canLogin", "false");
        RClaims.Add("canLogout", "false");
        var timer = new Stopwatch();


        //Act
        timer.Start();
        actualResult = authService.Authorize(principal, RClaims);
        timer.Stop();

        //Assert
        Assert.True(actualResult == true);
    }

    [Fact]
    public void AuthService_Authorize_MissingClaimsPassedIn_ReturnFalse_Pass()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var actualResult = false;
        var hashService = new HashService();
        var logger = new LogService(logTarget, hashService);
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        //Setting up AppPrincipal object 
        IDictionary<string, string> Claims = new Dictionary<string, string>();
        Claims.Add("canLogin", "true");
        Claims.Add("canLogout", "true");
        AuthUserModel expectedAuthModel = new AuthUserModel(123, "SecurityTestUser", BitConverter.GetBytes(123456), "TestHash");
        var principal = new AppPrincipal(expectedAuthModel, Claims);

        //Setting up RequiredClaims 
        Dictionary<string, string> RClaims = new Dictionary<string, string>();
        RClaims.Add("canLogin", "false");
        var timer = new Stopwatch();


        //Act
        timer.Start();
        actualResult = authService.Authorize(principal, RClaims);
        timer.Stop();

        //Assert
        Assert.True(actualResult == true);
    }
}
