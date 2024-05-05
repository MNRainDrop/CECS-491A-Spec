using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary.Interfaces
{
    public interface IBuyTarget
    {
        IResponse SetBuyRequest(IBuyRequest request);
        IResponse GetSentBuyRequests(long uid);
        IResponse GetToMeBuyRequests(long uid);
        IResponse GetMatchingBuyRequest(IBuyRequest request);
        IResponse UpdateBuyRequest(IBuyRequest updatedRequest);
        IResponse RemoveBuyRequest(IBuyRequest request);
    }
}
