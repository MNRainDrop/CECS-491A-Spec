using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface IAccountCreationManager
    {
        IResponse CallVerifyUser(string email);
        bool IsValidAccountType(string accountType);
        bool IsValidDateOfBirth(DateTime dateOfBirth);
        IResponse RegisterUser(string username, DateTime dateOfBirth, string accountType);
    }
}