using TeamSpecs.RideAlong.Model;
namespace TeamSpecs.RideAlong.UserAdministration;

public interface IUserAdministrationService
{
    IResponse CreateValidUserAccount(string userName, string dateOfBirth);
    IResponse DeleteUserAccount(string userName);
}
