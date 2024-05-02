using System.Security.Claims;
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
    }

    public IResponse DeleteUserClaim(IAccountUserModel user, string claim)
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
    }

    public IResponse ModifyUserClaim(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims)
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
    }
}
