using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface IAccountCreationManager
    {
        IResponse CallVerifyUser(string email);
        IResponse RegisterUser(IProfileUserModel profile, string email, string OTP, string accountType);
    }
}