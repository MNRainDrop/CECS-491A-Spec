using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using System.Security.Cryptography;
using System.Text;

namespace TeamSpecs.RideAlong.UserAdministration;

/// <summary>
/// 
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
    public IResponse CreateValidUserAccount(string userName, string dateOfBirth)
    {
        DateTime DOB;
        #region Validating arguments
        if(String.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException($"{nameof(userName)} must be valid");
        }
        if(String.IsNullOrWhiteSpace(dateOfBirth))
        {
            throw new ArgumentException($"{nameof(dateOfBirth)} must be valid");
        }
        if(!DateTime.TryParse(dateOfBirth, out DOB))
        {
            throw new ArgumentException($"{nameof(dateOfBirth)} must be valid date");
        }
        #endregion

        IResponse response = new Response();
        var userAccount = new AccountUserModel(userName);

        // Create User Hash
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(userName);
            var userPepper = _pepperService.RetrievePepper("UserAdministration");
            //var userHash = sha256Hash.ComputeHash(bytes + userPepper);
            //userAccount.UserHash = userHash.ToString();
        }

        // Create OTP Hash
        using (SHA256 sha256Hash = SHA256.Create())
        {
            
            byte[] bytes = Encoding.UTF8.GetBytes(userName);
            var OTPPepper = _pepperService.RetrievePepper("UserAdministration");
            var OTPSalt = _randomService.GenerateUnsignedInt();
            //var OTPHash = sha256Hash.ComputeHash(bytes + OTPPepper + OTPSalt);

            //userAccount.OTPHash = OTPHash.ToString();
        }

        // Generate user default claims
        var userClaims = GenerateDefaultClaims();

        // Write user to data store
        response = _userTarget.CreateUserAccountSql(userAccount, userClaims);

        // 
        return response;
    }

    private IDictionary<string, string> GenerateDefaultClaims()
    {
        IDictionary<string, string> claims = new Dictionary<string, string>();
        claims.Add(new KeyValuePair<string, string>("CanLogin", "True"));
        return claims;
    }
}
