using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary.Interfaces
{
    public interface IPartsTarget
    {
        // Parts
        IResponse SetCarPart(ICarPart part);
        IResponse GetUserParts(long uid);
        IResponse GetMatchingParts(List<ICarPart> part);
        IResponse RemoveParts(ICarPart part);
        // Listings
        IResponse SetListing(IListing listing);
        IResponse GetPartListing(ICarPart part);
        IResponse AmendListing(IListing updatingListing);
        IResponse RemoveListing(ICarPart part);

    }
}
