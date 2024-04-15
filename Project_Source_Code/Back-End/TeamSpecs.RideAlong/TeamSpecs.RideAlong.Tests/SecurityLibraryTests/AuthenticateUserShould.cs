using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.TestingLibrary;
#pragma warning disable
public class AuthenticateUserShould
{
    private string GenerateRandomHash()
    {
        string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVQXYZ0123456789";
        int length = 64; //Length of a hash is 64
        StringBuilder sb = new StringBuilder(length);
        Random random = new Random();
        for (int i = 0; i < length; i++)
        {
            int index = random.Next(AllowedChars.Length);
            sb.Append(AllowedChars[index]);
        }
        return sb.ToString();
    }

    [Fact]
    public void Authenticate_A_Username_pass()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var hashService = new HashService();
        var logger = new LogService(logTarget, hashService);
        var authRequest = new AuthNRequest("UserName", "OTP");
        var otp = "OTP";
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        var timer = new Stopwatch();
        var expectedResult = true;
        var actualResult = false;

        //Act
        timer.Start();
        actualResult = authService.Authenticate(authRequest, otp);
        timer.Stop();

        //Assert
        Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void GetUserModel_FromDatabase_Pass()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var hashService = new HashService();
        var logger = new LogService(logTarget, hashService);
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        var timer = new Stopwatch();
        var expectedResult = true;
        var actualResult = false;

        IResponse response;
        IAuthUserModel authModel;

        //Act
        timer.Start();
        response = authService.GetUserModel("sample_user@gmail.com");
        if (response.ReturnValue is not null)
        {
            //Check if we got what we ex
            authModel = (AuthUserModel)response.ReturnValue.First();
            AuthUserModel expectedAuthModel = new AuthUserModel(123, "sample_user@gmail.com", BitConverter.GetBytes(123456), "sample_user_hash");//The UID doesn't matter here
            if (expectedAuthModel.userName.Equals(authModel.userName) & expectedAuthModel.salt.SequenceEqual(authModel.salt) & expectedAuthModel.userHash.Equals(authModel.userHash))
            { actualResult = true; }
        }
        timer.Stop();

        //Assert
        Assert.Equal(expectedResult, actualResult);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

    [Fact]
    public void Not_GetUserPrincpal_if_No_Such_User_Exists_Fail()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var hashService = new HashService();
        var logger = new LogService(logTarget, hashService);
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        var timer = new Stopwatch();
        var expectedResult = false;
        var actualResult = false;
        IResponse response;
        IAuthUserModel authModel;

        //Act
        timer.Start();
        response = authService.GetUserModel("SecurityTestUser");
        var errorMessage = response.ErrorMessage;
        if (response.ReturnValue is not null)
        {
            //Check if we got what we ex
            authModel = (AuthUserModel)response.ReturnValue.First();
            AuthUserModel expectedAuthModel = new AuthUserModel(123, "SecurityTestUser", BitConverter.GetBytes(123456), GenerateRandomHash());//The UID doesn't matter here
            if (expectedAuthModel.userName.Equals(authModel.userName) & expectedAuthModel.salt.SequenceEqual(authModel.salt) & expectedAuthModel.userHash.Equals(authModel.userHash))
            { actualResult = true; }
        }
        timer.Stop();

        //Assert
        Assert.Equal(expectedResult, actualResult);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
    }

    [Fact]
    public void getUserPrincipal()
    {
        //Arrange
        var dao = new SqlServerDAO();
        var logTarget = new SqlDbLogTarget(dao);
        var hashService = new HashService();
        var logger = new LogService(logTarget, hashService);
        var authRequest = new AuthNRequest("UserName", "OTP");
        var otp = "OTP";
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        var timer = new Stopwatch();
        var expectedResult = true;
        var actualResult = false;
        IResponse response;
        // Note: this uses the ridealong sample user set, for a default user with one car

        //Act
        IAuthUserModel model = (AuthUserModel)(authService.GetUserModel("sample_user@gmail.com")).ReturnValue.First();
        timer.Start();

        response = authService.GetUserPrincipal(model);
        if (response.HasError != true)
        {
            if (response.ReturnValue is not null)
            {
                if (response.ReturnValue.First() is IAppPrincipal)
                {
                    actualResult = true;
                }
            }
        }

        timer.Stop();

        //Assert
        Assert.True(actualResult == expectedResult);
        Assert.True(timer.Elapsed.TotalSeconds < 3);

    }

    [Fact]
    public void confirmIdTokensAreValid_Pass()
    {
        var expected = true;
        var actual = false;
        string _rideAlongSecretKey = "This is Ridealong's super secret key for testing security";
        string _rideAlongIssuer = "Ride Along by Team Specs";

        var dao = new SqlServerDAO();
        var hasher = new HashService();
        var logTarget = new SqlDbLogTarget(dao);
        var logger = new LogService(logTarget, hasher);
        var authTarget = new SQLServerAuthTarget(dao, logger);
        var authService = new AuthService(authTarget, logger);
        var httpContext = new HttpContextAccessor();
        var sm = new SecurityManager(authService, logger, httpContext);
        var name = "sample_user@gmail.com";

        var modelResponse = authService.GetUserModel(name);
        var model = (AuthUserModel)modelResponse.ReturnValue.First();
        var principalResponse = authService.GetUserPrincipal(model);
        var principal = (RideAlongPrincipal)principalResponse.ReturnValue.First();

        var IdTokenResponse = sm.CreateIdToken(principal, DateTime.UtcNow);
        var token = (string)IdTokenResponse.ReturnValue.First();


        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_rideAlongSecretKey);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _rideAlongIssuer,
                ValidateAudience = false,
            }, out SecurityToken validatedToken);
            actual = true;
        }
        catch (Exception ex)
        {
            actual = false;
        }

        Assert.Equal(expected, actual);
    }
}
#pragma warning restore