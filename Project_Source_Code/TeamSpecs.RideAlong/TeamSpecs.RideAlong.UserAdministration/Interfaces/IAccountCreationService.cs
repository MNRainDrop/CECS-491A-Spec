using System.Security.Principal;
using TeamSpecs.RideAlong.Model;
namespace TeamSpecs.RideAlong.UserAdministration;

public interface IAccountCreationService
{
    IResponse CreateValidUserAccount(string userName, DateTime dateOfBirth, string accountType);

    IResponse IsUserRegistered (string userName);
}
