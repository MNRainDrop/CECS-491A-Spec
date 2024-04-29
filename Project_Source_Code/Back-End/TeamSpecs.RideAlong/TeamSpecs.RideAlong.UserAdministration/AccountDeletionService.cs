using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.LoggingLibrary;

namespace TeamSpecs.RideAlong.UserAdministration;

public class AccountDeletionService : IAccountDeletionService
{
    private readonly IUserTarget _userTarget;
    private readonly ILogService _logService;
    public AccountDeletionService(IUserTarget userTarget, ILogService logService)
    {
        _userTarget = userTarget;
        _logService = logService;
    }
    public IResponse DeleteUserAccount(IAccountUserModel userAccount)
    {
        #region Validate arguments
        if (string.IsNullOrWhiteSpace(userAccount.UserName))
        {
            _logService.CreateLogAsync("Error", "Data", "Invalid Data Provided", userAccount.UserHash);
            throw new ArgumentException($"{nameof(userAccount.UserName)} must be valid");
        }
        #endregion



        var response = _userTarget.DeleteUserAccountSql(userAccount.UserName);

        // Validate Response
        if (response.HasError)
        {
            response.ErrorMessage = "Could not Delete account";
        }
        else
        {
            response.ErrorMessage = "Successful";
        }

        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Account Deletion", response.ErrorMessage, userAccount.UserHash);


        return response;
    }
}
