using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration;

public interface IUserTarget
{
    IResponse GetUpdatedUserSql(IAccountUserModel userModel);
    IResponse PostUpdatedUserSql(IAccountUserModel userAccount, string address, string name, string phone, string accountType);
}