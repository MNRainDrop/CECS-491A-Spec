
using Microsoft.AspNetCore.Authentication;
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
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Model;
using Org.BouncyCastle.Crypto.Macs;
using System.Data.SqlTypes;
using TeamSpecs.RideAlong.Services;
using System.Collections;

namespace TeamSpecs.RideAlong.UserAdministration.Managers
{
    public class AccountCreationManager : IAccountCreationManager
    {
        private readonly IAccountCreationService _accountCreationService;
        private readonly ILogService _logService;
        private readonly IAuthService _authService;
        private readonly IHashService _hashService;
        private readonly IClaimService _claimService;

        public AccountCreationManager(IAccountCreationService accountCreationService, ILogService logService, IAuthService authService, 
            IHashService hashService, IClaimService claimService)
        {
            _accountCreationService = accountCreationService;
            _logService = logService;
            _authService = authService;
            _hashService = hashService;
            _claimService = claimService;
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
            #region Varaibles 
            IResponse response = new Response();
            var timer = new Stopwatch();
            bool otpMatch = false;
            IAuthUserModel authUser = new AuthUserModel();
            IAccountUserModel modelUser = new AccountUserModel(email);
            #endregion

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
                response.ErrorMessage = "User entered invalid account type";
                response.HasError = true;
                return response;
            }

            #endregion

            #region Retrieving UserModel 
            // If all arguements are valid - Get User Model to retrieve OTP
            response = new Response();
            response = _authService.GetUserModel(email);
            
            if (response.HasError || (response.ReturnValue is not null && response.ReturnValue.Count == 0))
            {
                response.HasError = true;
                return response;
            }
            // Need authUser/ modelUser to call ClaimService & AuthService
            if (response.ReturnValue is not null && response.ReturnValue.ToList()[0] is IAuthUserModel model)
            {
                authUser.UID = model.UID;
                authUser.userName = email;
                authUser.salt = model.salt;
                authUser.userHash = model.userHash;
                modelUser.UserId = model.UID;
                modelUser.UserName = email;
                modelUser.Salt = BitConverter.ToUInt32(authUser.salt);
                modelUser.UserHash = model.userHash;
            }

            #endregion

            #region Converting OTP to hash/ Retrieving OtpHash in DB
            response = _authService.GetOtpHash(authUser);
            var otpHash = _hashService.hashUser(otp, BitConverter.ToInt32(authUser.salt));
            
            if (response.HasError || (response.ReturnValue is not null && response.ReturnValue.Count == 0))
            {
                response.HasError = true;
                return response;
            }
            // Comprasion of both hashes
            if (response.ReturnValue is not null && response.ReturnValue.ToList()[0] is string str)
            {
                otpMatch = (str == otpHash);
            }
            #endregion

            #region Generate Claims if OTP's match
            if (otpMatch)
            {
                if (accountType == "Default User")
                {
                    timer.Start();
                    response = _claimService.CreateUserClaim(modelUser,GenerateDefaultClaims());
                    timer.Stop();
                }
                else if (accountType == "Vendor")
                {
                    timer.Start();
                    response = _claimService.CreateUserClaim(modelUser,GenerateVendorClaims());
                    timer.Stop();
                }
                else
                {
                    timer.Start();
                    response = _claimService.CreateUserClaim(modelUser, GenerateRentalClaims());
                    timer.Stop();
                }

                //if timer > 3 seconds -- log 

                //if time > 10 seconds -- log

                if(response.HasError)
                {
                    response.ErrorMessage = "Claim generation failed.";
                    // log claim
                    return response;
                }

                response = _accountCreationService.createUserProfile(email, profile);

                if(response.HasError || (response.ReturnValue is not null && response.ReturnValue.Count() == 0))
                {
                    // If error occurs with userProfile, delete users existing claims
                    _claimService.DeleteAllUserClaims(modelUser);
                    _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + response.ErrorMessage, modelUser.UserHash);
                    return response;
                }


            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = "OTP's do not match";
                _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + response.ErrorMessage, modelUser.UserHash);
                return response;
            }
            #endregion

            response.HasError = false;
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

        private IList<Tuple<string, string>> GenerateDefaultClaims()
        {
            var list = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("canLogin", "true"),
                new Tuple<string, string>("canRequestCarHealthRating", "true"),
                new Tuple<string, string>("canCreateVehicle", "true"),
                new Tuple<string, string>("canView", "default"),
                new Tuple<string, string>("canView", "vehicleProfile"),
                new Tuple<string, string>("canView", "marketplace"),
                new Tuple<string, string>("canView", "serviceLog"),
                new Tuple<string, string>("ownsVehicle", "true"),
                new Tuple<string, string>("CanModifyServiceLog", "true"),
                new Tuple<string, string>("CanDeleteServiceLog", "true"),
                new Tuple<string, string>("CanCreateServiceLog", "true"),
                new Tuple<string, string>("canDeleteAccount", "true")
            };
            // ^^^ above needs to be edited to correlate to actual permissions

            return list;
        }

        private IDictionary<string, string> GenerateVendorClaims()
        {
            IDictionary<string, string> claims = new Dictionary<string, string>()
        {
            { "0", "True" },
            { "1", "True" },
            { "2", "True" },
            { "3", "True" },
            { "4", "True" },
            { "5", "True" },
            { "6", "True" },
            { "7", "True" },
            { "8", "True" },
            { "9", "True" },
            { "10", "True" }
        };

            // ^^^ above needs to be edited to correlate to actual permissions

            return claims;
        }

        private IDictionary<string, string> GenerateRentalClaims()
        {
            IDictionary<string, string> claims = new Dictionary<string, string>()
        {
            { "0", "True" },
            { "1", "True" },
            { "2", "True" },
            { "3", "True" },
            { "4", "True" },
            { "5", "True" },
            { "6", "True" },
            { "7", "True" },
            { "8", "True" },
            { "9", "True" },
            { "10", "True" }
        };

            // ^^^ above needs to be edited to correlate to actual permissions

            return claims;
        }

        private int getIntFromBitArray(BitArray bitArray)
        {

            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];

        }
    }
}
