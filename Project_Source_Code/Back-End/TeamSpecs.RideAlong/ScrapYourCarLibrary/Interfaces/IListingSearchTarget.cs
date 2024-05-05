using ScrapYourCarLibrary.Models;
using TeamSpecs.RideAlong.Model;

namespace ScrapYourCarLibrary.Interfaces
{
    public interface IListingSearchTarget
    {
        IResponse GetListingsBySearch(ISearchParameters searchBy);
    }
}
