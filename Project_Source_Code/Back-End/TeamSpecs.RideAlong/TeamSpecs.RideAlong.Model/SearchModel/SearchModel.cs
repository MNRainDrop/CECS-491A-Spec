namespace TeamSpecs.RideAlong.Model;

public class SearchModel : ISearchModel
{
    public ICollection<object> SearchParameters { get; set; }

    public SearchModel(ICollection<object> searchParameters)
    {
        SearchParameters = searchParameters;
    }
}
