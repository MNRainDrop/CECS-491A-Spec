using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;
public class ClaimService : IClaimService
{
    private readonly IClaimTarget _claimTarget;
    public ClaimService(IClaimTarget claimTarget)
    {
        _claimTarget = claimTarget;
    }

    public IResponse CreateUserClaim(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims)
    {
        #region Validate Arguments
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (string.IsNullOrWhiteSpace(user.UserHash))
        {
            throw new ArgumentNullException(nameof(user.UserHash));
        }
        if (claims is null)
        {
            throw new ArgumentNullException(nameof(claims));
        }
        #endregion

        #region Call Service and Return Response
        return _claimTarget.CreateClaimSQL(user, claims);
        #endregion
    }

    public IResponse CreateUserClaim(IAccountUserModel user, IList<Tuple<string, string>> claims)
    {
        #region Validate Arguments
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (string.IsNullOrWhiteSpace(user.UserHash))
        {
            throw new ArgumentNullException(nameof(user.UserHash));
        }
        if (claims is null)
        {
            throw new ArgumentNullException(nameof(claims));
        }
        #endregion

        #region Call Service and Return Response
        return _claimTarget.CreateClaimSQL(user, claims);
        #endregion
    }

    public IResponse DeleteAllUserClaims(IAccountUserModel user)
    {
        #region Validate Arguments
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (string.IsNullOrWhiteSpace(user.UserHash))
        {
            throw new ArgumentNullException(nameof(user.UserHash));
        }
        #endregion

        #region Call Service and Return Response
        return _claimTarget.DeleteAllUserClaimsSQL(user);
        #endregion
    }

    public IResponse DeleteUserClaim(IAccountUserModel user, string claim, string? scope = null)
    {
        #region Validate Arguments
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (string.IsNullOrWhiteSpace(user.UserHash))
        {
            throw new ArgumentNullException(nameof(user.UserHash));
        }
        if (string.IsNullOrWhiteSpace(claim))
        {
            throw new ArgumentNullException(nameof(claim));
        }
        #endregion

        #region Call Service and Return Response
        return _claimTarget.DeleteUserClaimSQL(user, claim, scope);
        #endregion
    }

    public IResponse ModifyUserClaim(IAccountUserModel user, KeyValuePair<string, string> currClaim, KeyValuePair<string, string> newClaim)
    {
        #region Validate Arguments
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (string.IsNullOrWhiteSpace(user.UserHash))
        {
            throw new ArgumentNullException(nameof(user.UserHash));
        }
        if (!newClaim.Key.Equals(currClaim.Key))
        {
            throw new InvalidDataException(nameof(newClaim.Key));
        }
        if (newClaim.Value is null)
        {
            throw new ArgumentNullException(nameof(newClaim.Value));
        }
        if (newClaim.Value.Equals(currClaim.Value) && newClaim.Key.Equals(currClaim.Key))
        {
            throw new Exception($"Duplicate claim: {newClaim.Key}, {newClaim.Value}");
        }
        #endregion

        #region Call Service and Return Response
        return _claimTarget.ModifyUserClaimSql(user, currClaim, newClaim);
        #endregion
    }
}
