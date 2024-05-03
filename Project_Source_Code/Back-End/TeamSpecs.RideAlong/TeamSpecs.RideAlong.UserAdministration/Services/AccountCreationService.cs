using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.UserAdministration.Services;

/// <summary>
/// Service that allows system to create users
/// </summary>
public class AccountCreationService : IAccountCreationService
{
    private readonly IUserTarget _userTarget;
    private readonly IPepperService _pepperService;
    private readonly IHashService _hashService;
    private readonly ILogService _logService;

    public AccountCreationService(IUserTarget userTarget, IPepperService pepperService, IHashService hashService, ILogService logService)
    {
        _userTarget = userTarget;
        _pepperService = pepperService;
        _hashService = hashService;
        _logService = logService;
    }

    public IResponse VerifyValidUserRegistered()
    {
        IResponse response = new Response();

        // Check inputs again

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

        // should recieve OTP, timestamp, UserAccount, UserProfile details

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
#pragma warning disable CS0168 // Variable is declared but never used
        IDictionary<int, string> userClaims;
#pragma warning restore CS0168 // Variable is declared but never used

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
