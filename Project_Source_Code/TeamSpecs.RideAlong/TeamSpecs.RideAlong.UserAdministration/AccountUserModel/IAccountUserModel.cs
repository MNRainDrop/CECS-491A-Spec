namespace TeamSpecs.RideAlong.UserAdministration;
public interface IAccountUserModel
{
    string UserName { get; set; }
    uint Salt { get; set; }
    string? UserHash { get; set; }
}