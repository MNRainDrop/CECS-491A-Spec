using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public interface IModifyClaimTarget
{
    IResponse ModifyUserClaimSql(IAccountUserModel user, KeyValuePair<string, string> currClaim, KeyValuePair<string, string> newClaim);
}