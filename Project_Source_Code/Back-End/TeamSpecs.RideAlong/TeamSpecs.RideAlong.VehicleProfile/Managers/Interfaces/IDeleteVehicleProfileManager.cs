using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IDeleteVehicleProfileManager
{
    IResponse DeleteVehicleProfile(IVehicleProfileModel vehicle, IAccountUserModel account);
}
