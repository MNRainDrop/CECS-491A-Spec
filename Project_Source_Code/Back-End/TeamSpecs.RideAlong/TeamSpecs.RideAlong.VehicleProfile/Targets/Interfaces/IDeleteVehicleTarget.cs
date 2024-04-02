using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IDeleteVehicleTarget
{
    public IResponse deleteVehicleProfileSql(IVehicleProfileModel vehicleProfile, IAccountUserModel userAccount);
}
