
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public interface IDeleteClaimService
{
    IResponse DeleteUserClaim(IAccountUserModel user, string claim, string? scope);
    IResponse DeleteAllUserClaims(IAccountUserModel user);
}
