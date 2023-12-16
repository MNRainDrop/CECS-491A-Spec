
namespace TeamSpecs.RideAlong.Model.AccountUserModel;
public class AccountUserModel : IAccountUserModel
{
    public string UserName { get; set;}
    public DateTimeOffset DateCreated { get; }
    public string? UserHash { get; set; } = null;
    public string? OTPHash { get; set; } = null;
    public IDictionary<string, string>? UserClaims { get; set; } = null;
    public DateTime DateOfBirth { get; set; }

    public AccountUserModel(string userName, DateTime dateOfBirth)
    {
        UserName = userName;
        DateOfBirth = dateOfBirth;
        DateCreated = DateTimeOffset.Now;
    }
}
