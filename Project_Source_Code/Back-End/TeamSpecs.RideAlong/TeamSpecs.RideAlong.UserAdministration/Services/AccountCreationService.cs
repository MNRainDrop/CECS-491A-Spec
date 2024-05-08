using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using static System.Net.WebRequestMethods;
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
        Subject: Your Registration Confirmation OTP

        Dear {email},

        Thank you for choosing to register with [Your Company/Platform Name]!

        To complete your registration and ensure the security of your account, we require you to verify your email address. Below is your One-Time Password (OTP):

        OTP: {otp}

        Please enter this OTP on the registration page to confirm your email address and finalize your registration process. This OTP is valid for 2 hours, so please ensure you complete the verification process promptly.

        If you didn't request this OTP or if you have any concerns about your account security, please contact our support team immediately!

        Thank you for choosing RideAlong. We look forward to serving you!
        
        Best regards,
        RideAlong Team";

        _mailKitService.SendEmail(email , "RideAlong Registration Confirmation", emailBody);


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

    public IResponse CreateValidUserAccount(string userName, DateTime dateOfBirth, string accountType)
    {

        #region Validate arguments
        if (string.IsNullOrWhiteSpace(userName))
        {
            _logService.CreateLogAsync("Error", "Data", "Invalid Data Provided", null);
            throw new ArgumentException($"{nameof(userName)} must be valid");
        }
        #endregion

        IResponse response = new Response();
        var userAccount = new AccountUserModel(userName);
        //var userProfile = new ProfileUserModel(dateOfBirth);
        IDictionary<int, string> userClaims;

        /*
        // Create User Hash
        var userPepper = _pepperService.RetrievePepper("AccountCreation");
        //userAccount.UserHash = _hashService.hashUser(userName, userPepper);

        // Use these lines of code while IPepperService and IHashService is not complete
        userAccount.UserHash = userName;

        // Create Salt
        var salt = RandomService.GenerateUnsignedInt();
        userAccount.Salt = salt;

        // Generate user default claims
        //var userClaims = GenerateDefaultClaims();

        // Write user to data store
        //response = _userTarget.CreateUserAccountSql(userAccount, userClaims);

        // Validate Response
        if (response.HasError)
        {
            response.ErrorMessage = "Could not create account";
        }
        else
        {
            response.HasError = false;
        }
        if (response.ErrorMessage == null)
        {
            response.ErrorMessage = "Successful";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.HasError ? response.ErrorMessage : "Successful", userAccount.UserHash);
        */

        // Return Response
        return response;
    }

    private IDictionary<int, string> GenerateDefaultClaims()
    {
        IDictionary<int, string> claims = new Dictionary<int, string>()
        {
            { 1, "True" },
            { 2, "True" }
        };

        return claims;
    }

    public int getDefaultClaimLength()
    {
        return GenerateDefaultClaims().Count;
    }
}
