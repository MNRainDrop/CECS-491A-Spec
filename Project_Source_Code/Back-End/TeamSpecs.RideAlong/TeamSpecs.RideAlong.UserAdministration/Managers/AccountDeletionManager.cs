using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

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
            var timer = new Stopwatch();

            // Disoassociate VP's from UID -- Set FleetManagement, MarketpalceStatus, & VendingStatus to default values
            timer.Start();
            response = _accountDeletionService.DeleteVehicles(model);
            timer.Stop();

            if(timer.ElapsedMilliseconds > 3000)
            {
                _logService.CreateLogAsync("Warning", "Business", "Deletion Service took longer than 3 seconds", model.UserHash);
            }
            else if(timer.ElapsedMilliseconds > 10000)
            {
                _logService.CreateLogAsync("Error", "Business", "AccountDeletionFailure: Deletion Service took longer than 10 seconds", model.UserHash);
            }

            // Delete all users claims
            _claimService.DeleteAllUserClaims(model);

            // Delete BuyRequest, Parts, Listings, Notificaton Center, UserDetails, UserProfile, OTP, UserAccount
            //implement here


            return response;
        }
    }
}
