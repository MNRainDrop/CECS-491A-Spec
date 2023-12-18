using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using System.Security.Cryptography;
using System.Text;

namespace TeamSpecs.RideAlong.UserAdministration;

/// <summary>
/// Service that allows system to create users
/// </summary>
public class AccountCreationService : IAccountCreationService
{
    private IUserTarget _userTarget;
    private IPepperService _pepperService;
    private IRandomService _randomService;
    
    public AccountCreationService(IUserTarget userTarget)
    {
        _userTarget = userTarget;
        _pepperService = new PepperService();
        _randomService = new RandomService();
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

        
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Create User Hash

            // Once the PepperService is finished, use this line of code
            //var userPepper = _pepperService.RetrievePepper("CreateAccount");

            // Use this line of code while IPepperService is not complete
            var userPepper = _randomService.GenerateUnsignedInt(32);

            byte[] bytes = Encoding.UTF8.GetBytes(string.Concat(userName, userPepper));
            var userHash = sha256Hash.ComputeHash(bytes);

            userAccount.UserHash = BitConverter.ToString(userHash).Replace("-", string.Empty);

            // Create OTP Hash
            // Note: SHA256 will be changed to another hashing algorithm for the OTP 

            // Once the PepperService is finished, use this line of code
            //var OTPPepper = _pepperService.RetrievePepper("OTP");

            // Use this line of code while IPepperService is not complete
            var OTPPepper = _randomService.GenerateUnsignedInt(32);
            
            var OTPSalt = _randomService.GenerateUnsignedInt(32);
            userAccount.OTPSalt = BitConverter.ToString(OTPSalt).Replace("-", string.Empty);
            bytes = Encoding.UTF8.GetBytes(string.Concat(userName, OTPSalt, OTPPepper));
            var OTPHash = sha256Hash.ComputeHash(bytes);

            userAccount.OTPHash = BitConverter.ToString(OTPHash).Replace("-", string.Empty);
        }

        // Generate user default claims
        var userClaims = GenerateDefaultClaims();

        // Write user to data store
        response = _userTarget.CreateUserAccountSql(userAccount, userClaims);

        // Validate Response
        if (response.HasError)
        {
            response.ErrorMessage = "Could not create account";
        }


        // Return Response
        return response;
    }

    private IDictionary<string, string> GenerateDefaultClaims()
    {
        IDictionary<string, string> claims = new Dictionary<string, string>()
        {
            { "CanLogin", "True" }
        };
        ;
        return claims;
    }
}
