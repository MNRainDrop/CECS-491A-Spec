using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public interface IDeleteClaimTarget
{
    IResponse DeleteUserClaimSQL(IAccountUserModel user, string claim);
    IResponse DeleteAllUserClaimsSQL(IAccountUserModel user);
}
