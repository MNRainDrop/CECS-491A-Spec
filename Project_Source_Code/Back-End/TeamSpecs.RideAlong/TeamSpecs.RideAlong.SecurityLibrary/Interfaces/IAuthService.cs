using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Model;



namespace TeamSpecs.RideAlong.SecurityLibrary.Interfaces
{
    public interface IAuthService
    {
        IResponse GetUserModel(string username);
        IResponse UpdateOtp(IAuthUserModel model, string otp);
        IResponse updateLoginAttempt(IAuthUserModel model, int attempts);
        IResponse GetUserPrincipal(IAuthUserModel model);
        IResponse GetOtpHash(IAuthUserModel model);
        IResponse GetUserLogInAttempts(IAuthUserModel model);
        IResponse GetFirstFailedLogin(IAuthUserModel model);
        IResponse SetFirstFailedLogin(IAuthUserModel model, DateTime datetime);
        bool Authenticate(AuthNRequest loginAttempt, string otpHash);
        bool Authorize(IAppPrincipal principal, Dictionary<string, string> requiredClaims);
    }
}
