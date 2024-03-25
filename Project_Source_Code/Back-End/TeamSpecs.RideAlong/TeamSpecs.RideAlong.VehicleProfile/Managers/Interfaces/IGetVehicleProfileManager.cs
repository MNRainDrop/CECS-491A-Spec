using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IGetVehicleProfilesManager
{
    IResponse GetVehicleProfiles(IAccountUserModel userAccount);
}
