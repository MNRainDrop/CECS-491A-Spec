using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.InventoryManagement;

public interface IRetrieveVendorVehicleService
{
    IResponse retrieveVendorVehicles(IAccountUserModel userAccount, int page, int itemsPerPage, ICollection<object>? searchFilters = null);
}