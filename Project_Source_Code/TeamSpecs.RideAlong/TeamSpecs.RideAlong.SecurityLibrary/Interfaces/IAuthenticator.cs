using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;

namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public interface IAuthenticator
    {
        Response GetUserModel(string userName);
        Response UpdateOTP(IAuthUserModel authUserModel);
        Response GetUserPrincipal(IAuthUserModel authUserModel);
        Response GetOTPHash(IAuthUserModel authUserModel);
        Response Authenticate(AuthenticationRequest authRequest, string hashedOTP);
        bool Authorize(IRideAlongPrincipal principal, Dictionary<string, string>[] requiredClaims);
    }
}
