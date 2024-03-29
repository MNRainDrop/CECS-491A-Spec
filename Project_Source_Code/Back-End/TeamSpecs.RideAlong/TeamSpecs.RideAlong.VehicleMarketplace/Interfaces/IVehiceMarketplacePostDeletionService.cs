using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleMarketplace;

public interface IVehiceMarketplacePostDeletionService
{
    IResponse DeletePostFromMarketplace(string VIN);
}
