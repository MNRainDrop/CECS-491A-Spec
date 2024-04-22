
namespace TeamSpecs.RideAlong.Model;

public class CharityModel : ICharityModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Link { get; set; }

    public CharityModel(string name, string description, string link)
    {
        Name = name;
        Description = description;
        Link = link;
    }
}
