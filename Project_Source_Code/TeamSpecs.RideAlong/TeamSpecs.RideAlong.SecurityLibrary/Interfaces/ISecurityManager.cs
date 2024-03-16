using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    internal interface ISecurityManager
    {
        Response StartLogin(string username);
        Response TryFinalizingLogin(string username, string otp);
        Response CreateIDToken();
        Response CreateAccessToken();
        Response RefreshToken();
        bool LogOut();
        bool Authorize();
        IAuthUserModel JwtToModel();
        IRideAlongPrincipal AccessTokenToPrincipal();
        string JwtToUsername(string jwt);
    }
}
