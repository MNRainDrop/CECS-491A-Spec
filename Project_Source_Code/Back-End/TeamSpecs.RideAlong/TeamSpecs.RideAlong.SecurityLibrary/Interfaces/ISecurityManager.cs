using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Model;
namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    public interface ISecurityManager
    {
        IResponse StartLogin(string username);
        IResponse TryAuthenticating(string username, string otp);
        IResponse CreateIdToken();
        IResponse CreateAccessToken();
        IResponse RefreshTokens();
        IResponse Logout();
        bool Authorize();
        RideAlongPrincipal JwtToPrincipal(string idJwt, string accessJwt);
        string GetUsernameFromJwt(string idJwt);
    }
}
