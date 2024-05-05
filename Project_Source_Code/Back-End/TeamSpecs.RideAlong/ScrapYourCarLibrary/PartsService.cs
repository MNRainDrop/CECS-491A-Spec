using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary
{
    public class PartsService : IPartsService
    {
        private IPartsTarget _target;
        public PartsService(IPartsTarget target)
        {
            _target = target;
        }

        public IResponse AddListingToPart(ICarPart part)
        {
            throw new NotImplementedException();
        }

        public IResponse CreateParts(List<ICarPart> parts)
        {
            throw new NotImplementedException();
        }

        public IResponse DeleteListings(List<IListing> listings)
        {
            throw new NotImplementedException();
        }

        public IResponse GetMatchingParts(ICarPart part)
        {
            throw new NotImplementedException();
        }

        public IResponse GetUserParts(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse RemoveParts(List<ICarPart> parts)
        {
            throw new NotImplementedException();
        }

        public IResponse RetrievePartListings(List<ICarPart> parts)
        {
            throw new NotImplementedException();
        }

        public IResponse RetrieveUserListings(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse UpdateListing(IListing listing)
        {
            throw new NotImplementedException();
        }
    }
}
