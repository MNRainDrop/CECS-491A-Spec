using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface ISqlDbUserDeletionTarget
    {
        IResponse DeleteUserAccount(string userName);
        IResponse DeleteVehicleProfiles(string userName);
    }
}