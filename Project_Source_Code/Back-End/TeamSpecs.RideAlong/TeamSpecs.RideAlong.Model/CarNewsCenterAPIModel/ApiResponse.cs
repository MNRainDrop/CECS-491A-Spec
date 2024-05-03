namespace TeamSpecs.RideAlong.Model
{
    public class ApiResponse : IApiResponse
    { 
        public int TotalArticles { get; set; }
        public List<Articles> Articles { get; set; } = new List<Articles>();
    }

}
