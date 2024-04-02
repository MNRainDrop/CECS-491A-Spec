using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface ICreateVehicleTarget
{
    IResponse CreateVehicleProfileSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails);
}
