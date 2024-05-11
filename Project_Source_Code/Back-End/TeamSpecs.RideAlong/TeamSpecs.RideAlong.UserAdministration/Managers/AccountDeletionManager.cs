using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.UserAdministration.Managers
{
    public class AccountDeletionManager
    {
        private readonly IAccountDeletionService _accountDeletionService;
        private readonly ILogService _logService;
        private readonly IClaimService _claimService;

        public AccountDeletionManager(IAccountDeletionService accountDeletionService, ILogService logService, IClaimService claimService) 
        {
            _accountDeletionService = accountDeletionService;
            _logService = logService;
            _claimService = claimService;
        }

        public IResponse DeleteUser(IAccountUserModel model)
        {
            IResponse response = new Response();

            // Disoassociate VP's from UID -- Set FleetManagement, MarketpalceStatus, & VendingStatus to default 
            //implement here

            // Delete BuyRequest, Parts, Listings, Notificaton Center
            //implement here

            // Delete UserDetails, UserProfile, OTP
            //implement here

            // Delete all users claims
            _claimService.DeleteAllUserClaims(model);

            // Delete UserAccount
            //implement here

            return response;
        }
    }
}
