using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Model
{
    public interface IApiResponse
    {
        int TotalArticles { get; set; }
        List<Articles> Articles { get; set; }
    }
}
