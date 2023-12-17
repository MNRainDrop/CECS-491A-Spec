using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration;

public interface IUserTarget
{
    IResponse CreateUserAccountSql(IAccountUserModel userModel, IDictionary<string, string> userClaims);
    IResponse DeleteUserAccountSql(string userName);
    IResponse ModifyUserProfileSql(string userName, IDictionary<string, string> something);
    IResponse RecoverUserAccountSql(string userName);
}