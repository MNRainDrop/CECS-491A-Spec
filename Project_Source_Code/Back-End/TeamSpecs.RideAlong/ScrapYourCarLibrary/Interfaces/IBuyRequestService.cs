using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary.Interfaces
{
    public interface IBuyRequestService
    {
        IResponse CreateBuyRequest(IBuyRequest request);
        IResponse RetrieveIncomingRequest(long uid);
        IResponse RetrieveOutgoingRequest(long uid);
        IResponse RetrieveMatchingRequest(IBuyRequest request);
        IResponse UpdateBuyRequest(IBuyRequest updatedRequest);
        IResponse DeleteBuyRequest(IBuyRequest request);
    }
}
