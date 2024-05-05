using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary.Interfaces
{
    public interface IPartsService
    {
        // Parts
        IResponse CreateParts(List<ICarPart> parts);
        IResponse GetUserParts(long uid);
        IResponse GetMatchingParts(ICarPart part);
        IResponse RemoveParts(List<ICarPart> parts);
        // Listings
        IResponse AddListingToPart(ICarPart part);
        IResponse RetrieveUserListings(long uid);
        IResponse RetrievePartListings(List<ICarPart> parts);
        IResponse UpdateListing(IListing listing);
        IResponse DeleteListings(List<IListing> listings);
    }
}

