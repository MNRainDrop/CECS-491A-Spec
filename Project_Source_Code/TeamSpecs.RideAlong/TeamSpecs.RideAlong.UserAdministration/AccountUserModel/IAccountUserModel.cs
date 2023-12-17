namespace TeamSpecs.RideAlong.UserAdministration;
public interface IAccountUserModel
{
    string UserName { get; set; }
    DateTimeOffset DateCreated { get; }
    string OTPSalt { get; set; }
    string UserHash { get; set; }
    string OTPHash { get; set; }
}