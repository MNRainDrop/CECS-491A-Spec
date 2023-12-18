using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration;

public interface IUserTarget
{
    IResponse CreateUserAccountSql(IAccountUserModel userModel, IDictionary<string, string> userClaims);
    IResponse DeleteUserAccountSql(string userName);
    IResponse ModifyUserProfileSql(string userName, IProfileUserModel profile);
    IResponse EnableUserAccountSql(string userName);
    IResponse DisableUserAccountSql(string userName);
    IResponse RecoverUserAccountSql(string userName);
}