using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Model
{
    public interface IArticles
    {
        string Title { get; set; }
        string Description { get; set; }
        string Content { get; set; }
        string Url { get; set; }
        string Image { get; set; }
        Source Source { get; set; }

    }
}
