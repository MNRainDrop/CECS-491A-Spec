namespace TeamSpecs.RideAlong.UserAdministration;
public class AccountUserModel : IAccountUserModel
{
    public string UserName { get; set;}
    public DateTimeOffset DateCreated { get; }
    public byte[]? OTPSalt { get; set; } = null;
    public byte[]? UserHash { get; set; } = null;
    public byte[]? OTPHash { get; set; } = null;
    
    public AccountUserModel(string userName)
    {
        UserName = userName;
        DateCreated = DateTimeOffset.Now;
    }
}
