using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface ICreateVehicleTarget
{
    IResponse createVehicleProfileSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails);
}
