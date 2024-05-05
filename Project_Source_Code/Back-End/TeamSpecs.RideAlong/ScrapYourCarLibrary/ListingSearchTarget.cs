using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.ScrapYourCarLibrary
{
    public class ListingSearchTarget : IListingSearchTarget
    {
        private IGenericDAO _dao;
        public ListingSearchTarget(IGenericDAO dao)
        {
            _dao = dao;
        }
        public IResponse GetListingsBySearch(ISearchParameters searchBy)
        {
            throw new NotImplementedException();
        }
    }
}
