using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IGetVehicleProfileDetailsManager
{
    IResponse GetVehicleProfileDetails(IVehicleProfileModel vehicleProfile, IAccountUserModel userAccount);
}
