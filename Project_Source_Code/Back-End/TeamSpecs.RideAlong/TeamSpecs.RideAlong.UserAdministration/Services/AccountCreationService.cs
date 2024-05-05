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
    private readonly int __otpLength;

    public AccountCreationService(ISqlDbUserCreationTarget userTarget, IPepperService pepperService, IHashService hashService, ILogService logService, IRandomService randomService)
    {
        _userTarget = userTarget;
        _pepperService = pepperService;
        _hashService = hashService;
        _logService = logService;
        _randomService = randomService;
        __otpLength = 10;
    }

    public IResponse verifyUser(string email)
    {
        IResponse response = new Response();

        response = _userTarget.CheckDbForEmail(email);

        if (response.HasError && response.ErrorMessage == "User exists in the Database")
        {
            _logService.CreateLogAsync("Info", "Debug", response.ErrorMessage, null);
            return response;
        }
        else if(response.HasError)
        {
            _logService.CreateLogAsync("Info", "Debug", response.ErrorMessage, null);
            response.ErrorMessage += " : CheckDbForExisitngEmail Failed.";
            return response;
        }

        #region Intializing variables and objects
        IAccountUserModel userAccount = new AccountUserModel(email);
        string otp;
        string otpHash;
        string emailBody;
        var userPepper = _pepperService.RetrievePepper("AccountCreation");
        var salt = _randomService.GenerateUnsignedInt();
        byte[] saltBytes = BitConverter.GetBytes(salt);
        #endregion

        #region Generate User Hash 
        userAccount.UserHash = _hashService.hashUser(email, (int)userPepper);
        userAccount.Salt = salt;
        #endregion

        #region Generate OTP & OTP Hash 
        otp = _randomService.GenerateRandomString(__otpLength);
        otpHash = _hashService.hashUser(otp, BitConverter.ToInt32(saltBytes));
        #endregion

        #region Send Email



        #endregion



        if (response.ErrorMessage == "User tables must be updated")
        {
            response.ErrorMessage = "";

            _userTarget.UpdateUserConfirmation();
        }

        // Check if this is how SQL would return no obj. --> No user exists
        if(response.ReturnValue == null)
        {
            // Gen OTP

            
            //string hashedUserAttempt = hasher.hashUser(otp, BitConverter.ToInt32(model.salt));

            
            // Create User Hash
            //var userPepper = _pepperService.RetrievePepper("AccountCreation");
            //userAccount.UserHash = _hashService.hashUser(userName, userPepper);


            // Create Salt
            //var salt = RandomService.GenerateUnsignedInt();
            //userAccount.Salt = salt;

            // HASH OTP

            // HASH email

            // 

            // response = _usertarget.
        }
        // If DB finds existing user
        else
        {
            response.HasError = true;
            response.ErrorMessage = "User exists in Database";
            return response;
        }

        return response;
    }

    public IResponse IsUserRegistered(string email)
    {
        IResponse response = new Response();

        // Check inputs again e.g ehitespace blah
        
        // send email to Sql Target

        // Create OTP 

        // Hash OTP

        // Timestamp of when OTP created 

        // store OTP, timestamp, UserModel, UserProfile in return value

        // Need to return the following:
        /*
         * Hashed OTP in Payload
         * Timestamp when OTP created in Payload
         * UserAccount, UserProfile Models
         */

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
