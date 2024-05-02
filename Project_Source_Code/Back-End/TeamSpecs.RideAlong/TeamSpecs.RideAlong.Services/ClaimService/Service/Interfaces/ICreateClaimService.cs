
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public interface ICreateClaimService
{
    IResponse CreateUserClaim(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims);
}