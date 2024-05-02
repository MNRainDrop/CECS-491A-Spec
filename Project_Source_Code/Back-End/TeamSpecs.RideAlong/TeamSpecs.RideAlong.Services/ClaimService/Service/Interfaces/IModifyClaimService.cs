
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public interface IModifyClaimService
{
    IResponse ModifyUserClaim(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims);
}
