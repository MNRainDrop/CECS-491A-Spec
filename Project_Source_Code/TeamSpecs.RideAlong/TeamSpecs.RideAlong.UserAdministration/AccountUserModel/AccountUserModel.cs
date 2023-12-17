namespace TeamSpecs.RideAlong.UserAdministration;
public class AccountUserModel : IAccountUserModel
{
    public string UserName { get; set;}
    public DateTimeOffset DateCreated { get; }
    public string? UserSalt { get; set; } = null;
    public string? UserHash { get; set; } = null;
    public string? OTPHash { get; set; } = null;
    
    public AccountUserModel(string userName)
    {
        UserName = userName;
        DateCreated = DateTimeOffset.Now;
    }
}
