using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IVehicleProfileDeletionService
{
    public IResponse DeleteVehicleProfile(IVehicleProfileModel vehicleProfile, IAccountUserModel userAccount);
}
