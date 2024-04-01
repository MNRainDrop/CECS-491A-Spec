using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface ICreateVehiclesTarget
{
    IResponse CreateVehicleProfileSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails);
}
