
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Services;

namespace TeamSpecs.RideAlong.UserAdministration.Managers
{
    public class AccountCreationManager : IAccountCreationManager
    {
        private readonly IAccountCreationService _accountCreationService;
        private readonly ILogService _logService;

        public AccountCreationManager(IAccountCreationService accountCreationService, ILogService logService)
        {
            _accountCreationService = accountCreationService;
            _logService = logService;
        }

        public IResponse CallVerifyUser(string email)
        {
            IResponse response = new Response();
            var timer = new Stopwatch();

            #region Validiate email 
            if (!IsValidEmail(email))
            {
                response.ErrorMessage = "User entered invalid email";
                _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + response.ErrorMessage, null);
                response.HasError = true;
                return response;
            }
            #endregion

            timer.Start();
            response = _accountCreationService.verifyUser(email);
            timer.Stop();

            #region Log to Database 
            if (timer.ElapsedMilliseconds > 3000)
            {
                _logService.CreateLog("Info", "Business", "AccountCreationFailure: " + "AccountCreationService: Exceeded 3 second time limit ", null);
            }

            if (response.HasError)
            {
                _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + response.ErrorMessage, null);
                return response;
            }
            #endregion

            return response;
        }

        public IResponse RegisterUser(IProfileUserModel profile, string email, string otp, string accountType)
        {
            IResponse response = new Response();
            var timer = new Stopwatch();

            // Check business rules in BRD 
            /* The following is needed to be checked
             * Username is a email --> aaa@something.com
             * DOB --> after 1/1/1970
             * Account Type must be valid account type
             */

            #region Business Rules
            if (!IsValidEmail(email))
            {
                response.ErrorMessage = "User entered invalid email.";
                _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + response.ErrorMessage, null);
                response.HasError = true;
                return response;
            }
            if(!IsValidDateOfBirth(profile.DateOfBirth))
            {
                response.ErrorMessage = "User entered invalid date of birth";
                _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + response.ErrorMessage, null);
                response.HasError = true;
                return response;
            }
            if(!IsValidAltEmail(profile.AlternateUserName))
            {
                response.ErrorMessage = "User entered invalid Alt. username";
                response.HasError = true;
                return response;
            }
            if(!IsValidOneTimePassword(otp))
            {
                response.ErrorMessage = "User entered invalid OTP";
                response.HasError = true;
                return response;
            }
            if (!IsValidAccountType(accountType))
            {
                response.ErrorMessage = "User entered account type";
                response.HasError = true;
                return response;
            }

            #endregion

            if(accountType == "Default User")
            {
                timer.Start();
                //response = _accountCreationService.
                timer.Stop();
            }
            else if(accountType == "Vendor")
            {
                timer.Start();
                //response = _accountCreationService.
                timer.Stop();
            }
            else
            {
                timer.Start();
                //response = _accountCreationService.
                timer.Stop();
            }



            return response;
        }

        private bool IsValidEmail(string email)
        {
            // Minimum length check
            if (email.Length < 3)
                return false;

            // Regular expression pattern for email validation
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // Check if the email matches the pattern
            return Regex.IsMatch(email, pattern);

        }

        private bool IsValidDateOfBirth(DateTime dateOfBirth)
        {
            // Get the current date
            DateTime currentDate = DateTime.Today;

            // Calculate the minimum allowed date of birth (18 years before the current date)
            DateTime minDateOfBirth = currentDate.AddYears(-18);

            // Calculate the maximum allowed date of birth (January 1, 1970)
            DateTime maxDateOfBirth = new DateTime(1970, 1, 1);

            // Check if the provided date of birth is within the valid range
            return dateOfBirth >= maxDateOfBirth && dateOfBirth <= minDateOfBirth;
        }

        private bool IsValidAltEmail(string email)
        {
            IResponse altEmailResponse = new Response();

            altEmailResponse = _accountCreationService.verifyAltUser(email);

            if (altEmailResponse.HasError)
            {
                _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + altEmailResponse.ErrorMessage, null);
                return false;
            }

            return true;
        }

        private bool IsValidAccountType(string accountType)
        {
            return accountType == "Vendor" || accountType == "Renter" || accountType == "Default User" || accountType == "Admin";
        }

        private bool IsValidOneTimePassword(string otp)
        {
            if(otp == null)
            {
                return false;
            }

            string pattern = @"^[a-zA-Z0-9]{10}$";

            return Regex.IsMatch(otp, pattern);
        }
    }
}
