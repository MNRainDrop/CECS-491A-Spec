using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IModifyVehicleTarget
{
    public IResponse ModifyVehicleProfileSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails);

    public IResponse UpdateVehicleOwnerSql(IVehicleProfileModel vehicleProfile, IVehicleDetailsModel vehicleDetails);
}
