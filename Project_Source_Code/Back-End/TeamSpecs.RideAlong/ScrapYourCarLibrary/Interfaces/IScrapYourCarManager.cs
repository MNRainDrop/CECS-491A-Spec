using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary.Interfaces
{
    public interface IScrapYourCarManager
    {
        // Part/Listing Section
        // Parts
        IResponse GenerateDefaultParts(string vin);
        IResponse GenerateParts(List<ICarPart> part);
        IResponse GetUserParts(long uid);
        IResponse DeleteParts(List<ICarPart> parts);
        // Listings
        IResponse GenenrateListing(IListing listings);
        IResponse GetUserListings(long uid);
        IResponse EditListing(IListing listing);
        IResponse DeleteListing(List<IListing> listings);
        // Search       Section
        IResponse SearchParts(ISearchParameters searchBy);
        // BuyRequests  Section
        IResponse CreateBuyRequest(IBuyRequest request);
        IResponse GetIncomingBuyRequest(IBuyRequest request);
        IResponse GetOutgoingBuyRequest(IBuyRequest request);
        IResponse UpdateBuyRequest(IBuyRequest request);
        IResponse DeleteBuyRequest(IBuyRequest request);
    }
}
