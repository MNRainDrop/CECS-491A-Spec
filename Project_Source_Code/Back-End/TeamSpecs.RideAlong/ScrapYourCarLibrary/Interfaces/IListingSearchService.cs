using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary.Interfaces
{
    public interface IListingSearchService
    {
        IResponse RetrieveListingsBySearch(ISearchParameters searchBy);
    }
}
