using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.ScrapYourCarLibrary
{
    public class BuyRequestService : IBuyRequestService
    {
        private IBuyTarget _target;
        public BuyRequestService(IBuyTarget target)
        {
            _target = target;
        }
        public IResponse CreateBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse DeleteBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse RetrieveIncomingRequest(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse RetrieveMatchingRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse RetrieveOutgoingRequest(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse UpdateBuyRequest(IBuyRequest updatedRequest)
        {
            throw new NotImplementedException();
        }
    }
}
