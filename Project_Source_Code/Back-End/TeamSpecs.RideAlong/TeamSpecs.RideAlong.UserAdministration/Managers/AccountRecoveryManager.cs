using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Services;
namespace TeamSpecs.RideAlong.UserAdministration.Managers
{
    public class AccountRecoveryManager
    {
        private readonly IAccountRecoveryService _accountRecoveryService;
        private readonly ILogService _logService;
        private readonly IAuthService _authService;
        private readonly IHashService _hashService;
        private readonly IClaimService _claimService;
        private readonly IPepperService _pepperService;
        private readonly IRandomService _randomService;
        private readonly IMailKitService _mailKitService;
        private readonly int __otpLength;

        public AccountRecoveryManager(IAccountRecoveryService accountRecoveryService, ILogService logService, 
            IAuthService authService,IHashService hashService, IClaimService claimService, IPepperService pepperService, 
            IRandomService randomService, IMailKitService mailKitService) 
        {
            _accountRecoveryService = accountRecoveryService;
            _logService = logService;
            _authService = authService;
            _hashService = hashService;
            _claimService = claimService;
            _pepperService = pepperService;
            _randomService = randomService;
            _mailKitService = mailKitService;
            __otpLength = 10;
        }

        public IResponse sendRecoveryEmail(string email)
        {
            #region Varaibles 
            IResponse response = new Response();
            var userPepper = _pepperService.RetrievePepper("RideAlongPepper");
            var userHash = _hashService.hashUser(email, (int)userPepper);
            string altEmail;
            string otp;
            string otpHash;
            string emailBody;
            byte[]? saltBytes = null;
            #endregion

            #region Validaite Arguements
            if (!IsValidEmail(email))
            {
                response.ErrorMessage = "User entered invalid email.";
                _logService.CreateLogAsync("Info", "Business", "AccountRecoveryFailure: " + response.ErrorMessage, userHash);
                response.HasError = true;
                return response;
            }
            #endregion

            #region get Alt. email 
            response = _accountRecoveryService.getUserRecoveryEmail(email);

            if (response.ErrorMessage is not null)
            {
                if (response.ErrorMessage.Contains("Could not"))
                {
                    _logService.CreateLogAsync("Info", "Business", "AccountRecoveryFailure: " + response.ErrorMessage, userHash);
                    return response;
                }
                else
                {
                    _logService.CreateLogAsync("Info", "Business", "AccountRecoveryFailure: " + response.ErrorMessage, userHash);
                    return response;
                }
            }
            
            if(response.ReturnValue is not null)
            {
                if (response.ReturnValue.ToList()[0] is object[] array)
                {
                    if(array[0].ToString() is string str)
                    {
                        altEmail = str;
                    }
                    if (array[1] is int salt)
                    {
                        saltBytes = BitConverter.GetBytes(salt);
                    }
                }
            }

            #endregion

            #region Set OTP Hash
            otp = _randomService.GenerateRandomString(__otpLength);
            otpHash = _hashService.hashUser(otp, BitConverter.ToInt32(saltBytes));
            #endregion

            #region Send Email with OTP 
            emailBody = $@"

                Dear {email},

                This email indicates you have issued a account recovery request!

                To complete your recovery and ensure the security of your account, we require you to verify your alternate email address before you add new email. Below is your One-Time Password (OTP):

                OTP: {otp}

                Please enter this OTP on the registration page to confirm your email address and finalize your registration process. This OTP is valid for 5 minutes, so please ensure you complete the verification process promptly.

                If you didn't request this OTP or if you have any concerns about your account security, please contact our support team immediately!
        
                Best regards,
                RideAlong Team";
            #endregion

            #region Attempt to send email 
            try
            {
                _mailKitService.SendEmail(email, "RideAlong Registration Confirmation", emailBody);
            }
            catch
            {
                response.HasError = true;
                response.ErrorMessage = " Emailing service failed";
                _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + response.ErrorMessage, userHash);
            }
            #endregion

            #region Upload new OTP 
            //response = _accountRecoveryService.setOTP();
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
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9.-]{1,}$";

            // Check if the email matches the pattern
            return Regex.IsMatch(email, pattern);

        }
    }
}
