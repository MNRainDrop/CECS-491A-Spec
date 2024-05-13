namespace TeamSpecs.RideAlong.Model;
public class UserDataRequestModel : IAccountUserModel
{
    public long UserId { get; set; }
    public string UserName { get; set; }
    public uint Salt { get; set; } = 0;
    public string? UserHash { get; set; } = null;
    public DateTime DoB { get; set; }
    public string? AltUserName { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public string? Address { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }

    public UserDataRequestModel(string userName)
    {
        UserName = userName;
    }
}
