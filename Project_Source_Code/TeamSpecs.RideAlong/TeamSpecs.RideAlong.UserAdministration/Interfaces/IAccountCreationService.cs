using TeamSpecs.RideAlong.Model;
namespace TeamSpecs.RideAlong.UserAdministration;

public interface IAccountCreationService
{
    IResponse CreateValidUserAccount(string userName);
}
