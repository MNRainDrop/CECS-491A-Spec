using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface IAccountDeletionService
    {
        IResponse DeleteUser(IAccountUserModel model);
        IResponse DeleteVehicles(IAccountUserModel model);
        IResponse CreateAccountDeletionRequestTable(string userHash);
    }
}