using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Model;



namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    public interface IAuthService
    {
        IResponse GetUserModel(string username);
        IResponse UpdateOtp(IAuthUserModel model, string otp);
        IResponse GetUserPrincipal(IAuthUserModel model);
        IResponse GetOtpHash(IAuthUserModel model);
        bool Authenticate(AuthNRequest loginAttempt, string otpHash);
        bool Authorize(IAppPrincipal, Dictionary<string, string> requiredClaims);
    }
}
