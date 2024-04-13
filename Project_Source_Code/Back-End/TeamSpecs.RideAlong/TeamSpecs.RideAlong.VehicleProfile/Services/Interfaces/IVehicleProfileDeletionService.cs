using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IVehicleProfileDeletionService
{
    public IResponse deleteVehicleProfile(IVehicleProfileModel vehicleProfile, IAccountUserModel userAccount);
}
