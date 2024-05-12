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
    public class AccountDeletionManager : IAccountDeletionManager
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

            #region Checking timer 
            if (timer.ElapsedMilliseconds > 3000)
            {
                _logService.CreateLogAsync("Warning", "Business", "Deletion Service took longer than 3 seconds", model.UserHash);
            }
            else if (timer.ElapsedMilliseconds > 10000)
            {
                _logService.CreateLogAsync("Error", "Business", "AccountDeletionFailure: Deletion Service took longer than 10 seconds", model.UserHash);
            }
            #endregion

            #region Checking if service failed 
            if (response.ErrorMessage is not null)
            {
                if (response.HasError && response.ErrorMessage.Contains("Could not"))
                {
                    _logService.CreateLogAsync("Info", "Business", "AccountDeletionFailure: " + response.ErrorMessage, model.UserHash);
                    return response;
                }
                else if (response.HasError)
                {
                    _logService.CreateLogAsync("Info", "Business", "AccountDeletionFailure: " + response.ErrorMessage, model.UserHash);
                    return response;
                }
            }
            #endregion

            timer.Restart();

            // Delete all users claims
            timer.Start();
            response = _claimService.DeleteAllUserClaims(model);
            timer.Stop();

            #region Checking timer 
            if (timer.ElapsedMilliseconds > 3000)
            {
                _logService.CreateLogAsync("Warning", "Business", "Claims Service took longer than 3 seconds", model.UserHash);
            }
            else if (timer.ElapsedMilliseconds > 10000)
            {
                _logService.CreateLogAsync("Error", "Business", "AccountDeletionFailure: Claims Service took longer than 10 seconds", model.UserHash);
            }
            #endregion

            #region Checking if Claims service failed 
            if (response.ErrorMessage is not null)
            {
                if (response.HasError && response.ErrorMessage.Contains("Could not"))
                {
                    _logService.CreateLogAsync("Info", "Business", "AccountDeletionFailure: " + response.ErrorMessage, model.UserHash);
                    return response;
                }
                else if (response.HasError)
                {
                    _logService.CreateLogAsync("Info", "Business", "AccountDeletionFailure: " + response.ErrorMessage, model.UserHash);
                    return response;
                }
            }
            if (response.ReturnValue is not null)
            {
                if (response.ReturnValue.Count == 0)
                {
                    response.HasError = true;
                    response.ErrorMessage = "Claims didn't update the neccesary amount of tables";
                    return response;
                }
            }
            #endregion

            timer.Restart();

            // Delete BuyRequest, Parts, Listings, Notificaton Center, UserDetails, UserProfile, OTP, UserAccount
            timer.Start();
            response = _accountDeletionService.DeleteUser(model);
            timer.Stop();

            #region Checking timer
            if (timer.ElapsedMilliseconds > 3000)
            {
                _logService.CreateLogAsync("Warning", "Business", "Claims Service took longer than 3 seconds", model.UserHash);
            }
            else if (timer.ElapsedMilliseconds > 10000)
            {
                _logService.CreateLogAsync("Error", "Business", "AccountDeletionFailure: Claims Service took longer than 10 seconds", model.UserHash);
            }
            #endregion

            #region Checking if Deletion service failed
            if (response.ErrorMessage is not null)
            {
                if (response.HasError && response.ErrorMessage.Contains("Could not"))
                {
                    _logService.CreateLogAsync("Info", "Business", "AccountDeletionFailure: " + response.ErrorMessage, model.UserHash);
                    return response;
                }
                else if (response.HasError)
                {
                    _logService.CreateLogAsync("Info", "Business", "AccountDeletionFailure: " + response.ErrorMessage, model.UserHash);
                    return response;
                }
            }
            #endregion

            response.HasError = false;
            return response;
        }
    }
}
