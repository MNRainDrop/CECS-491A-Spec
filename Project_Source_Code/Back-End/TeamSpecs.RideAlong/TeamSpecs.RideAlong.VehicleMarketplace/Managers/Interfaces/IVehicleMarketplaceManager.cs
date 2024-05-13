using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleMarketplace;

public interface IVehicleMarketplaceManager
{
    IResponse CreateVehicleProfilePost(string VIN, int view, string Description, int MarketplaceStatus);

    IResponse DeletePostFromMarketplace(string VIN);

    IResponse RetrieveAllPublicPost(int page);
    IResponse SendBuyRequest(long uid, string vin, int price);

    IResponse RetrieveDetailVehicleProfile(string VIN);


}
