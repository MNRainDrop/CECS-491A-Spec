using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration;

public class SqlDbUserTarget : IUserTarget
{
    public IResponse CreateUserAccountSql(IAccountUserModel userModel, IDictionary<string, string> userClaims)
    {
        throw new NotImplementedException();
    }
}