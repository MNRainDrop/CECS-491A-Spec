using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.ScrapYourCarLibrary
{
    public class BuyTarget : IBuyTarget
    {
        private IGenericDAO _dao;
        public BuyTarget(IGenericDAO dao)
        {
            _dao = dao;
        }
        public IResponse GetMatchingBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse GetSentBuyRequests(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse GetToMeBuyRequests(long uid)
        {
            throw new NotImplementedException();
        }

        public IResponse RemoveBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse SetBuyRequest(IBuyRequest request)
        {
            throw new NotImplementedException();
        }

        public IResponse UpdateBuyRequest(IBuyRequest updatedRequest)
        {
            throw new NotImplementedException();
        }
    }
}
