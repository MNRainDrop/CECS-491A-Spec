using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration.Managers
{
    public class AccountClaimManager : IAccountClaimManager
    {
        private readonly ILogService _logService;
        private readonly IAuthService _authService;
        private readonly IClaimService _claimService;

        public AccountClaimManager(ILogService logService, IAuthService authService, IClaimService claimService)
        {
            _logService = logService;
            _authService = authService;
            _claimService = claimService;
        }
        public IResponse DisableUser(IAccountUserModel user)
        {
            IResponse response;
            response = _claimService.DeleteAllUserClaims(user);
            return response;
        }

        public IResponse CreateUserClaim(IAccountUserModel user, IList<Tuple<string, string>> claims) {
            IResponse response;
            response = _claimService.CreateUserClaim(user,claims);
            return response;
        }

        public IResponse CreateUserClaim(IAccountUserModel user, ICollection<KeyValuePair<string, string>> claims)
        {
            IResponse response;
            response = _claimService.CreateUserClaim(user, claims);
            return response;
        }
    }
}
