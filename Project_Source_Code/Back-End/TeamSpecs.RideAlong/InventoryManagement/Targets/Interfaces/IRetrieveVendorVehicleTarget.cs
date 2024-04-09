using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.InventoryManagement;

public interface IRetrieveVendorVehicleTarget
{
    IResponse readVendorVehicleProfiles(ICollection<object> searchParameters, int numOfResults, int page);
}
