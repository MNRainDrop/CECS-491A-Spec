using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.InventoryManagement;

public interface ISortVendorVehiclesTarget
{
    IResponse sortVendorVehicleProfiles(ICollection<object> searchParameters, int numOfResults, int page);
}
