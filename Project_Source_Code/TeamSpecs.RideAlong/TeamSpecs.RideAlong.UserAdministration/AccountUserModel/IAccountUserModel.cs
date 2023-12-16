namespace TeamSpecs.RideAlong.Model.AccountUserModel;
public interface IAccountUserModel
{
    string UserName { get; set; }
    DateTimeOffset DateCreated { get; }
    string UserHash { get; set; }
    string OTPHash { get; set; }
    IDictionary<string, string> UserClaims { get; set; }
}