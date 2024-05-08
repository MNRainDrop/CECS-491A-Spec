using TeamSpecs.RideAlong.Model;
namespace TeamSpecs.RideAlong.VehicleMarketplace;

public interface IVehiceMarketplaceSendBuyRequestService
{
    IResponse SendBuyRequest(string vin, int price);
}

