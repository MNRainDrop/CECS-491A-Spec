using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public interface IModifyClaimTarget
{
    IResponse ModifyUserClaimSql(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims);
}