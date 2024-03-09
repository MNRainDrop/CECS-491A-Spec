using TeamSpecs.RideAlong.Model;
namespace TeamSpecs.RideAlong.UserAdministration;

public interface IAccountDeletionService
{
    IResponse DeleteUserAccount(IAccountUserModel userAccount);
}
