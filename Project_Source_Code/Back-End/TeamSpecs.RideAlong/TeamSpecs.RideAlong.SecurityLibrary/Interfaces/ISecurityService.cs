using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Model;


namespace TeamSpecs.RideAlong.SecurityLibrary
{
    public interface ISecurityService
    {
        AuthUserModel GetUserModel(string username);
        Response UpdateOtp(AuthUserModel model);
        Response GetUserPrincipal(AuthUserModel model);
        Response GetOtpHash(AuthUserModel model);
        bool Authenticate(AuthRequest loginAttempt, string otpHash);
        bool Authorize(RideAlongPrincipal principal, Dictionary<string, string> requiredClaims);
    }
}
