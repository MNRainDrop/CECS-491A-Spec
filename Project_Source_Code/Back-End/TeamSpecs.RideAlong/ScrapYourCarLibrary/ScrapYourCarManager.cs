using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.ScrapYourCarLibrary
{
    public class ScrapYourCarManager : IScrapYourCarManager
    {
        private IPartsService _pService;
        private IBuyRequestService _bService;
        private IListingSearchService _lService;
        public ScrapYourCarManager(IPartsService pService, IBuyRequestService bService, IListingSearchService lService)
        {
            _pService = pService;
            _bService = bService;
            _lService = lService;
        }

        public IResponse CreateBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse DeleteBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse DeleteListing(List<IListing> listings)
        {
            throw new NotImplementedException();
        }

        public IResponse DeleteParts(List<ICarPart> parts)
        {
            throw new NotImplementedException();
        }

        public IResponse EditListing(IListing listing)
        {
            throw new NotImplementedException();
        }

        public IResponse GenenrateListing(IListing listings)
        {
            throw new NotImplementedException();
        }

        public IResponse GenerateDefaultParts(string vin)
        {
            throw new NotImplementedException();
        }

        public IResponse GenerateParts(List<ICarPart> part)
        {
            throw new NotImplementedException();
        }

        public IResponse GetIncomingBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse GetOutgoingBuyRequest(IBuyRequest request)
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

        public IResponse SearchParts(ISearchParameters searchBy)
        {
            throw new NotImplementedException();
        }

        public IResponse UpdateBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
