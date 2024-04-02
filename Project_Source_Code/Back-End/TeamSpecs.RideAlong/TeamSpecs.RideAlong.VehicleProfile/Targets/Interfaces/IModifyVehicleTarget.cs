using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IModifyVehicleTarget
{
    public IResponse modifyVehicleProfileSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails);
}
