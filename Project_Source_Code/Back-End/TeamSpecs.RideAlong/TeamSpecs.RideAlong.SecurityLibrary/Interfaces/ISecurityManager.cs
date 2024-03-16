using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Model;
namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    public interface ISecurityManager
    {
        Response StartLogin(string username);
        Response TryAuthenticating(string username, string otp);
        Response CreateIdToken();
        Response CreateAccessToken();
        Response RefreshTokens();
        Response Logout();
        bool Authorize();
        RideAlongPrincipal JwtToPrincipal(string idJwt, string accessJwt);
        string GetUsernameFromJwt(string idJwt);
    }
}
