using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public interface IGetVehicleCountTarget
{
    IResponse GetVehicleCount(IAccountUserModel userAccount);
}
