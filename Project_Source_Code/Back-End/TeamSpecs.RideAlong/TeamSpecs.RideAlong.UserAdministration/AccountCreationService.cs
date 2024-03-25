using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.LoggingLibrary;

namespace TeamSpecs.RideAlong.UserAdministration;

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
    public IResponse CreateValidUserAccount(string userName)
    {
        #region Validate arguments
        if(string.IsNullOrWhiteSpace(userName))
        {
            _logService.CreateLogAsync("Error", "Data", "Invalid Data Provided", null);
            throw new ArgumentException($"{nameof(userName)} must be valid");
        }
        #endregion

        IResponse response;
        var userAccount = new AccountUserModel(userName);


        // Create User Hash

        // Once the IPepperService and IHashService is finished, use these line of code
        //var userPepper = _pepperService.RetrievePepper("CreateAccount");
        //userAccount.UserHash = _hashService.hashUser(userName, userPepper);

        // Use these lines of code while IPepperService and IHashService is not complete
        userAccount.UserHash = userName;
        
        // Create Salt
        var salt = RandomService.GenerateUnsignedInt();
        userAccount.Salt = salt;

        // Generate user default claims
        var userClaims = GenerateDefaultClaims();

        // Write user to data store
        response = _userTarget.CreateUserAccountSql(userAccount, userClaims);

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
