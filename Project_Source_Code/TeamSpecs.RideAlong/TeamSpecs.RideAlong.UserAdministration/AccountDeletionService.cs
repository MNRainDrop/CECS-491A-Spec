using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.DataAccess;

namespace TeamSpecs.RideAlong.UserAdministration;

public class AccountDeletionService : IAccountDeletionService
{
    private IUserTarget _userTarget;
    private ILogService _logService;
    public AccountDeletionService(IUserTarget userTarget)
    {
        _userTarget = userTarget;
        _logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()));
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

        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Account Deletion", response.HasError ? response.ErrorMessage : "Successful", userAccount.UserHash);


        return response;
    }
}
