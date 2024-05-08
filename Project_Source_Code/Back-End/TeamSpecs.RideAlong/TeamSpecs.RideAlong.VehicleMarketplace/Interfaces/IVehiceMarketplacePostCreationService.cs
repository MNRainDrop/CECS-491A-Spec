using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleMarketplace;
public interface IVehiceMarketplacePostCreationService
{
    IResponse CreateVehicleProfilePost(string VIN, int view, string Description, int MarketplaceStatus);
}

