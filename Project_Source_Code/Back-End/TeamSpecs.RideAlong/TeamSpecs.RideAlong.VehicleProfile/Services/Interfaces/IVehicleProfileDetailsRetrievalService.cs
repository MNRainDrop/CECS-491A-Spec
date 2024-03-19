using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IVehicleProfileDetailsRetrievalService
{
    IResponse retrieveVehicleDetails(IVehicleProfileModel vehicleProfile, IAccountUserModel userAccount);
}
