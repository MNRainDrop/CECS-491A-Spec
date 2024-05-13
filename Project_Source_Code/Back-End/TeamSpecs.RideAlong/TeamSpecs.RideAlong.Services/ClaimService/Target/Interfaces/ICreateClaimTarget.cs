using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public interface ICreateClaimTarget
{
    IResponse CreateClaimSQL(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims);

    IResponse CreateClaimSQL(IAccountUserModel user, IList<Tuple<string, string>> claims);
}
