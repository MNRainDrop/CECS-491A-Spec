using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IVehicleProfileModificationService
{
    IResponse ModifyVehicleProfile(IVehicleProfileModel vehicle, IVehicleDetailsModel vehicleDetails, IAccountUserModel userAccount);
}
