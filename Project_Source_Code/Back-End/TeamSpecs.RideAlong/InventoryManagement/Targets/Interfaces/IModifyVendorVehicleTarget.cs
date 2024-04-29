using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.InventoryManagement;

public interface IModifyVendorVehicleTarget
{
    IResponse modifyVendorVehicleProfileSql(IVendorVehicleModel vehicle, IAccountUserModel userAccount);
}
