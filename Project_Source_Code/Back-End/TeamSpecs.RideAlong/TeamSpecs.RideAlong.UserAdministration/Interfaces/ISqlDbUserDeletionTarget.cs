using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface ISqlDbUserDeletionTarget
    {
        IResponse DeleteUserAccount(long uid);
        IResponse DeleteVehicleProfiles(long uid);
    }
}