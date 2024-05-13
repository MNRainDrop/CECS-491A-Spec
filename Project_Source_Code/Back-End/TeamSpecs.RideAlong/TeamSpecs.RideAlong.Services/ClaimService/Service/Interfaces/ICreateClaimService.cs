
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public interface ICreateClaimService
{
    IResponse CreateUserClaim(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims);

    IResponse CreateUserClaim(IAccountUserModel user, IList<Tuple<string, string>> claims);
}