using ScrapYourCarLibrary.Interfaces;
using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.ScrapYourCarLibrary
{
    public class ListingSearchService : IListingSearchService
    {
        private IListingSearchTarget _target;
        public ListingSearchService(IListingSearchTarget target)
        {
            _target = target;
        }
        public IResponse RetrieveListingsBySearch(ISearchParameters searchBy)
        {
            throw new NotImplementedException();
        }
    }
}
