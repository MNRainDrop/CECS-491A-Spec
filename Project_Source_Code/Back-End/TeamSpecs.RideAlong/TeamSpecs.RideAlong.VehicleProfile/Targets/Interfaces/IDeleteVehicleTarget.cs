using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IDeleteVehicleTarget
{
    public IResponse DeleteVehicleProfileSql(IVehicleProfileModel vehicleProfile, IAccountUserModel userAccount);
}
