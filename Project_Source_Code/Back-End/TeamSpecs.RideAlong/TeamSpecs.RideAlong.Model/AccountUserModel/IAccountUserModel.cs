namespace TeamSpecs.RideAlong.Model;
public interface IAccountUserModel
{
    long UserId { get; set; }
    string UserName { get; set; }
    long Salt { get; set; }
    string? UserHash { get; set; }
}