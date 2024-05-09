﻿using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary.Interfaces
{
    public interface IPartsService
    {
        // Parts
        IResponse CreateParts(List<ICarPart> parts);
        IResponse GetUserParts(long uid);
        IResponse GetMatchingParts(List<ICarPart> parts);
        IResponse RemoveParts(List<ICarPart> parts);
        // Listings
        IResponse AddListingToPart(IListing listing);
        IResponse RetrieveUserListings(long uid);
        IResponse RetrievePartListings(List<ICarPart> parts);
        IResponse UpdateListing(IListing listing);
        IResponse DeleteListings(List<IListing> listings);
    }
}
