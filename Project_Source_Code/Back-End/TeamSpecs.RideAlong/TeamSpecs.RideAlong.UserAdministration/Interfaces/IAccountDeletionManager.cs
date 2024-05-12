using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface IAccountDeletionManager
    {
        IResponse DeleteUser(IAccountUserModel model);
    }
}