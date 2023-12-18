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
    public IResponse CreateValidUserAccount(string userName)
    {
        #region Validate arguments
        if(string.IsNullOrWhiteSpace(userName))
        {
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

        _logService.CreateLog(response.HasError ? "Error" : "Info", "Account Creation", response.HasError ? response.ErrorMessage : "Successful", userAccount.UserHash);
        

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
