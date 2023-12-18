namespace TeamSpecs.RideAlong.UserAdministration;
public interface IAccountUserModel
{
    string UserName { get; set; }
    DateTimeOffset DateCreated { get; }
    byte[] OTPSalt { get; set; }
    byte[] UserHash { get; set; }
    byte[] OTPHash { get; set; }
}