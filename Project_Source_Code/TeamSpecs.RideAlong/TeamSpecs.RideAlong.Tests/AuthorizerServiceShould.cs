using System.Diagnostics;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.UserAdministration;

namespace TeamSpecs.RideAlong.TestingLibrary;

public class AuthorizeUserShould
{
    [Fact]
    public void AuthService_IsAuthorize_RequiredClaimsPassedIn_ReturnTrue_Pass()
    {
        //Arrange 
        var timer = new Stopwatch(); 
        IResponse response;
        var authorizationObject = new AuthService();

        IDictionary<string, string> claims = new Dictionary<string, string>();
        claims.Add("CanLogIn", "Yes");
        var testUserName = "test@gmail.com";
        var userModel = new AccountUserModel(testUserName);
        var CurrentPrincipal = new AppPrincipal(userModel, claims);

        IDictionary<string, string> Rclaims = new Dictionary<string, string>();
        Rclaims.Add("CanLogIn", "Yes");


        //Act 
        timer.Start();
        bool result = authorizationObject.IsAuthorize(CurrentPrincipal, Rclaims);
        timer.Stop();

        //Assert 
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(result); 
        
    }
}
