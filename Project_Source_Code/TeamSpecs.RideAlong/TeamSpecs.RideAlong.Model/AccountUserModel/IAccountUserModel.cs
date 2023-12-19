namespace TeamSpecs.RideAlong.Model;
public interface IAccountUserModel
{
    string UserName { get; set; }
    uint Salt { get; set; }
    string? UserHash { get; set; }
}