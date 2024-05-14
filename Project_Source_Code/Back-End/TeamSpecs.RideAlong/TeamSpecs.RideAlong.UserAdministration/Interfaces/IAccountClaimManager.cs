using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.UserAdministration.Interfaces
{
    public interface IAccountClaimManager
    {
        IResponse DisableUser(IAccountUserModel user);

        IResponse CreateUserClaim(IAccountUserModel user, IList<Tuple<string, string>> claims);

        IResponse CreateUserClaim(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims);
    }
}