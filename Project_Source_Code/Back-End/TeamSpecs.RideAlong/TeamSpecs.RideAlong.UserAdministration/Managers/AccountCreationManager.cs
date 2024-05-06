using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

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

            #region Validiate email 
            if (!IsValidEmail(email))
            {
                response.ErrorMessage = "User entered invalid email";
                _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + response.ErrorMessage, null);
                response.HasError = true;
                return response;
            }
            #endregion

            response = _accountCreationService.verifyUser(email);

            if (response.HasError)
            {
                _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + response.ErrorMessage, null);
                return response;
            }

            return response;
        }

        // Rename to verifying account details
        // No longer creates account in DB due to needing confirm account first
        public IResponse RegisterUser(IProfileUserModel profile, string email, string otp, string accountType)
        {
            IResponse response = new Response();

            // Check business rules in BRD 
            /* The following is needed to be checked
             * Username is a email --> aaa@something.com
             * DOB --> after 1/1/1970
             * Account Type must be valid account type
             */

            #region Business Rules
            // Check if email is valid

            #region Validiate emails
            if (!IsValidEmail(email))
            {
                response.ErrorMessage = "User entered invalid email. Email altered in payload.";
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

            #endregion

            // Check if date of birth is valid



            // Call account creation service
            //response = _accountCreationService.CreateValidUserAccount(username, dateOfBirth, accountType);

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
            #endregion

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

            //altEmailResponse = _accountCreationService.

            return true;
        }
        private bool IsValidAccountType(string accountType)
        {
            return accountType == "Vendor" || accountType == "Renter" || accountType == "Default User";
        }
    }
}
