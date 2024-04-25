namespace TeamSpecs.RideAlong.VehicleProfile;
using TeamSpecs.RideAlong.Model;

public interface ICreateVehicleProfileManager
{
    IResponse CreateVehicleProfile(IVehicleProfileModel vehicle, IVehicleDetailsModel vehicleDetails, IAccountUserModel account);
}
