using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary
{
    public class PartsTarget : IPartsTarget
    {
        private IGenericDAO _dao;
        public PartsTarget(IGenericDAO dao)
        {
            _dao = dao;
        }
        public IResponse AmendListing(IListing updatingListing)
        {
            throw new NotImplementedException();
        }

        public IResponse GetMatchingParts(List<ICarPart> part)
        {
            throw new NotImplementedException();
        }

        public IResponse GetPartListing(ICarPart part)
        {
            throw new NotImplementedException();
        }

        public IResponse GetUserListings(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse GetUserParts(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse RemoveListing(IListing listing)
        {
            throw new NotImplementedException();
        }

        public IResponse RemoveParts(ICarPart part)
        {
            throw new NotImplementedException();
        }

        public IResponse SetCarPart(ICarPart part)
        {
            throw new NotImplementedException();
        }

        public IResponse SetListing(IListing listing)
        {
            throw new NotImplementedException();
        }
    }
}
