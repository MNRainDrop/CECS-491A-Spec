using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Model
{
    public interface ISource
    {
        string Name { get; set; }
        string Url { get; set; }
    }
}
