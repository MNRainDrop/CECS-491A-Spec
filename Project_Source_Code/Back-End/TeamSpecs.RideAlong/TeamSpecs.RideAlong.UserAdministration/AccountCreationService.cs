using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.DataAccess;

namespace TeamSpecs.RideAlong.UserAdministration;

/// <summary>
/// Service that allows system to create users
/// </summary>
public class AccountCreationService : IAccountCreationService
{
    private IUserTarget _userTarget;
    private IPepperService _pepperService;
    private IHashService _hashService;
    private ILogService _logService;
    
    public AccountCreationService(IUserTarget userTarget, IPepperService pepperService, IHashService hashService)
    {
        _userTarget = userTarget;
        _pepperService = pepperService;
        _hashService = hashService;
        _logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()));
    }
    public IResponse CreateValidUserAccount(string userName, DateTime dateOfBirth, string accountType)
    {
        #region Validate arguments
        if(string.IsNullOrWhiteSpace(userName))
        {
            _logService.CreateLogAsync("Error", "Data", "Invalid Data Provided", null);
            throw new ArgumentException($"{nameof(userName)} must be valid");
        }
        if(dateOfBirth == null)
        {
            _logService.CreateLogAsync("Error", "Data", "Invalid Data Provided", null);
            throw new ArgumentException($"{nameof(dateOfBirth)} must be not null");
        }
        #endregion

        IResponse response;
        var userAccount = new AccountUserModel(userName);
        var userProfile = new ProfileUserModel(dateOfBirth);
        IDictionary<int, string> userClaims;

        // Create User Hash
        var userPepper = _pepperService.RetrievePepper("AccountCreation");
        userAccount.UserHash = _hashService.hashUser(userName, userPepper);
        
        // Create Salt stand in
        var salt = RandomService.GenerateUnsignedInt();
        userAccount.Salt = salt;

        // Encountered issue of Int32 being too low or high in Sql Generation
        if (userAccount.Salt > Int32.MaxValue || userAccount.Salt < UInt32.MinValue)
        {
            userAccount.Salt = 12067862;
        }

        #region Checking AccountType
        if (accountType == "Vendor")
        {
            userClaims = GenerateVendorClaims();
        }
        if(accountType == "Rental Fleet")
        {
            userClaims = GenerateRentalFleetClaims();
        }
        else
        {
            userClaims = GenerateDefaultClaims();
        }
        #endregion

        // Write user to data store
        response = _userTarget.CreateUserAccountSql(userAccount, userProfile, userClaims);

        #region Validiate Response
        if (response.HasError)
        {
            response.ErrorMessage = "Could not create account";
        }
        else
        {
            response.HasError = false;
        }
        #endregion

        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server",                                           
    response.HasError ? response.ErrorMessage : "Successful",  // Message
    response.HasError ? null : userAccount.UserHash       // UserHash assc. with Log (UserHash or null)
    );
        


        // Return Response
        return response;
    }

    public IResponse IsUserRegistered(string userName)
    {
        IResponse response;

        response = _userTarget.CheckIfUserAccountExistsSql(userName);

        //log

        return response;
    }

    private IDictionary<int, string> GenerateDefaultClaims()
    {
        IDictionary<int, string> claims = new Dictionary<int, string>()
        {
            { 1, "True" },
            { 2, "True" },
            { 3, "True" }
        };
        
        return claims;
    }

    private IDictionary<int, string> GenerateVendorClaims()
    {
        IDictionary<int, string> claims = new Dictionary<int, string>()
        {
            { 1, "True" },
            { 2, "True" },
            { 3, "False" }
        };

        return claims;
    }

    private IDictionary<int, string> GenerateRentalFleetClaims()
    {
        IDictionary<int, string> claims = new Dictionary<int, string>()
        {
            { 1, "True" },
            { 2, "False" },
            { 3, "False" }
        };

        return claims;
    }
    public int getDefaultClaimLength()
    {
        return GenerateDefaultClaims().Count;
    }
}
