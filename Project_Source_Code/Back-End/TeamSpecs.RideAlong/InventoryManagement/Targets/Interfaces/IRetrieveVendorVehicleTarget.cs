using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.InventoryManagement;

public interface IRetrieveVendorVehicleTarget
{
    IResponse readVendorVehicleProfilesSql(ICollection<object> searchParameters, int numOfResults, int page);
}
