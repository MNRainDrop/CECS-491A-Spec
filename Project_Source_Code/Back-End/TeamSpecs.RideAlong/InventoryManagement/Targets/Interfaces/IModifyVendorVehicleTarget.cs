using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.InventoryManagement;

public interface IModifyVendorVehicleTarget
{
    IResponse modifyVendorVehicleProfile(IVendorVehicleModel vehicle, IAccountUserModel userAccount);
}
