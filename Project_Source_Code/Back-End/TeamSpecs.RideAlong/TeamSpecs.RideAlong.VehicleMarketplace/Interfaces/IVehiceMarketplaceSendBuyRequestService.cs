using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleMarketplace;

public interface IVehiceMarketplaceSendBuyRequestService
{
    IResponse SendBuyRequest(long uid, string vin, int price);
}

