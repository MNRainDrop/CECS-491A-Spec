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
        private IResponse createErrorResponse(Exception ex)
        {
            IResponse errorResponse = new Response();
            errorResponse.HasError = true;
            errorResponse.ErrorMessage = "Error retrieving search: " + ex.Message;
            return errorResponse;
        }
        public IResponse RetrieveListingsBySearch(ISearchParameters searchBy)
        {
            try
            {
                return _target.GetListingsBySearch(searchBy);
            }
            catch (Exception ex)
            {
                return createErrorResponse(ex);
            }


        }
    }
}
