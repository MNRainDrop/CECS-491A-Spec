using System.Diagnostics;
using System.Text.RegularExpressions;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration.Managers
{
    public class AccountUpdateManager : IAccountUpdateManager
    {
        private IPostAccountUpdateService _PostaccountUpdateService;
        private IGetAccountUpdateService _GetaccountUpdateService;
        private readonly ILogService _logService;
        public AccountUpdateManager(IPostAccountUpdateService accountUpdateService, IGetAccountUpdateService getAccountUpdateService, ILogService logService)
        {
            _PostaccountUpdateService = accountUpdateService;
            _GetaccountUpdateService = getAccountUpdateService;
            _logService = logService;
        }

        public IResponse PostAccountUpdate(IAccountUserModel userAccount, string address, string name, string phone, string accountType)
        {
            #region Business Rules
            if (!IsValidAddress(address))
            {
                throw new ArgumentException(nameof(address));
            }

            if (!IsValidUsername(name))
            {

                throw new ArgumentException(nameof(name));
            }

            if (!IsValidPhone(phone))
            {
                throw new ArgumentException(nameof(phone));
            }

            if (!IsValidAccountType(accountType))
            {
                throw new ArgumentException(nameof(accountType));
            }
            #endregion

            IResponse response = new Model.Response();
            var timer = new Stopwatch();
            try
            {
                timer.Start();
                response = _PostaccountUpdateService.UpdateUserAccount(userAccount, address, name, phone, accountType);
                timer.Stop();
            }
            catch (Exception ex)
            {
                response.ErrorMessage += ex.Message;
                response.HasError = true;
            }

            if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
            {
                _logService.CreateLogAsync("Warning", "Server", "Creating Updated User took longer than 3 seconds, but less than 10. " + response.ErrorMessage, userAccount.UserHash);
            }
            if (timer.Elapsed.TotalSeconds > 10)
            {
                _logService.CreateLogAsync("Error", "Server", "Server Timeout on Account Update Service. " + response.ErrorMessage, userAccount.UserHash);
            }

            if (response.HasError)
            {
                response.ErrorMessage = "Could not update user account. " + response.ErrorMessage;
            }
            else
            {
                response.ErrorMessage = "Successfully update user account. " + response.ErrorMessage;
            }
            _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
            return response;
        }
        #region Validate Business Rules
        public bool IsValidAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return true;
            }

            if (!address.Contains("CA"))
            {
                return false;
            }

            return true;
        }
        public bool IsValidUsername(string name)
        {
            // Check if the name is empty
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            // Check if the name contains any digits
            if (Regex.IsMatch(name, @"\d"))
            {
                return false;
            }

            // Check if the name contains only alpha characters
            if (Regex.IsMatch(name, @"^[a-zA-Z]+$"))
            {
                return false;
            }

            // Check if the name length is less than 50 characters
            if (name.Length > 51)
            {
                return false;
            }

            return true;
        }

        public bool IsValidPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return true;
            }

            string noSpaces = phone.Replace(" ", "");

            if (Regex.IsMatch(noSpaces, @"[^0-9\(\)-]+"))
            {
                return false;
            }

            // Remove any non-digit characters from the phone number
            string digitsOnly = Regex.Replace(phone, @"\D", "");

            // Check if the resulting string contains 10 digits (North American format)
            if (digitsOnly.Length != 10)
            {
                return false;
            }

            return true;
        }

        public bool IsValidAccountType(string accountType)
        {
            return accountType == "Vendor" || accountType == "Renter" || accountType == "Default User";
        }
        #endregion


        public IResponse GetAccountUpdate(IAccountUserModel userAccount)
        {
            IResponse response = new Response();
            var timer = new Stopwatch();

            timer.Start();
            response = _GetaccountUpdateService.GetUpdateUserAccount(userAccount);
            timer.Stop();

            if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
            {
                _logService.CreateLogAsync("Warning", "Server", "Getting Updated User took longer than 3 seconds, but less than 10. " + response.ErrorMessage, userAccount.UserHash);
            }
            if (timer.Elapsed.TotalSeconds > 10)
            {
                _logService.CreateLogAsync("Error", "Server", "Server Timeout on Getting Account Update Service. " + response.ErrorMessage, userAccount.UserHash);
            }

            if (response.HasError == true) // Means that SQL generation/ DB  failed
            {
#pragma warning disable CS8604 // Possible null reference argument.
                _logService.CreateLogAsync("Error", "Server", response.ErrorMessage, userAccount.UserHash);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            else if (response.HasError == false)
            {
                _logService.CreateLogAsync("Info", "Business", "Successful retrieval of account update details", userAccount.UserHash);
            }

            return response;
        }
    }


}
