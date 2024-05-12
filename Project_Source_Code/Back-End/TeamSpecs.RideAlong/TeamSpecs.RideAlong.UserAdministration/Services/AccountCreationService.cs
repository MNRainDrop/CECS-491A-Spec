using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using System.Collections.Generic;

namespace TeamSpecs.RideAlong.UserAdministration.Services;

/// <summary>
/// Service that allows system to create users
/// </summary>
public class AccountCreationService : IAccountCreationService
{
    private readonly ISqlDbUserCreationTarget _userTarget;
    private readonly IPepperService _pepperService;
    private readonly IHashService _hashService;
    private readonly ILogService _logService;
    private readonly IRandomService _randomService;
    private readonly IMailKitService _mailKitService;
    private readonly int __otpLength;

    public AccountCreationService(ISqlDbUserCreationTarget userTarget, IPepperService pepperService, 
        IHashService hashService, ILogService logService, IRandomService randomService, IMailKitService mailKitService)
    {
        _userTarget = userTarget;
        _pepperService = pepperService;
        _hashService = hashService;
        _logService = logService;
        _randomService = randomService;
        _mailKitService = mailKitService;
        __otpLength = 10;
    }

    public IResponse verifyUser(string email)
    {
        IResponse response = new Response();
        var userPepper = _pepperService.RetrievePepper("RideAlongPepper");
        var userHash = _hashService.hashUser(email, (int)userPepper);

        response = _userTarget.CheckDbForEmail(email);

        if (response.HasError && response.ErrorMessage == "User exists in the Database")
        {
            _logService.CreateLogAsync("Info", "Business", "AccountCreationFailure: " + response.ErrorMessage, userHash);
            return response;
        }
        else if(response.HasError)
        {
            _logService.CreateLogAsync("Info", "Data Store","AccountCreationFailure: " + response.ErrorMessage, userHash);
            response.ErrorMessage += " : CheckDbForExisitngEmail target Failed.";
            return response;
        }

        #region Intializing variables and objects
        IAccountUserModel userAccount = new AccountUserModel(email);
        string otp;
        string otpHash;
        string emailBody;
        
        var salt = _randomService.GenerateUnsignedInt();
        byte[] saltBytes = BitConverter.GetBytes(salt);

        #endregion

        #region Set User Hash 
        userAccount.UserHash = userHash;
        userAccount.Salt = salt; 
        #endregion

        #region Generate OTP & OTP Hash 
        otp = _randomService.GenerateRandomString(__otpLength);
        otpHash = _hashService.hashUser(otp, BitConverter.ToInt32(saltBytes));
        #endregion

        #region Send Email

        emailBody = $@"

        Dear {email},

        Thank you for choosing to register with RideAlong!

        To complete your registration and ensure the security of your account, we require you to verify your email address. Below is your One-Time Password (OTP):

        OTP: {otp}

        Please enter this OTP on the registration page to confirm your email address and finalize your registration process. This OTP is valid for 2 hours, so please ensure you complete the verification process promptly.

        If you didn't request this OTP or if you have any concerns about your account security, please contact our support team immediately!

        Thank you for choosing RideAlong. We look forward to serving you!
        
        Best regards,
        RideAlong Team";

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

        #endregion

        #region Update User Table
        if (response.ErrorMessage == "User tables must be updated")
        {
            response.ErrorMessage = "";
            
            response = _userTarget.UpdateUserConfirmation(userAccount, otpHash);

            if(response.HasError)
            {
                _logService.CreateLogAsync("Info", "Data Store", "AccountCreationFailure: " + response.ErrorMessage, userHash);
                return response;
            }
            else
            {
                _logService.CreateLogAsync("Info", "Business", 
                    "AccountCreationSuccess: User sucessfully updated confirmation tables", userHash);
                return response;
            }
        }
        #endregion

        response = _userTarget.CreateUserConfirmation(userAccount, otpHash);

        if (response.HasError)
        {
            _logService.CreateLogAsync("Info", "Data Store", "AccountCreationFailure: " + response.ErrorMessage, userHash);
            return response;
        }

        return response;
    }

    public IResponse verifyAltUser(string email)
    {
        IResponse response = new Response();

        response = _userTarget.CheckDbForEmail(email);

        return response;
    }

    public IResponse createUserProfile(string userName, IProfileUserModel profile)
    {

        #region Validate arguments
        if (string.IsNullOrWhiteSpace(userName))
        {
            _logService.CreateLogAsync("Error", "Data", "Invalid Data Provided", null);
            throw new ArgumentException($"{nameof(userName)} must be valid");
        }
        #endregion

        IResponse response = new Response();
        var userPepper = _pepperService.RetrievePepper("RideAlongPepper");
        var userHash = _hashService.hashUser(userName, (int)userPepper);

        response = _userTarget.CreateUserProfile(userName, profile);

        if(response.HasError || (response.ReturnValue is not null && response.ReturnValue.Count == 0))
        {
            response.ErrorMessage = "Can't create create User Profile sql";
            return response;
        }


        response.HasError = false;
        return response;
    }

}
