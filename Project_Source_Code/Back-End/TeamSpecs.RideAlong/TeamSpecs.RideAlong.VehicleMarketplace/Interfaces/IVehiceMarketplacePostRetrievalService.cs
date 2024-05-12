using TeamSpecs.RideAlong.Model;
namespace TeamSpecs.RideAlong.VehicleMarketplace;

public interface IVehiceMarketplacePostRetrievalService
{
    IResponse RetrieveAllPublicPost(int numOfResults, int page);
}

