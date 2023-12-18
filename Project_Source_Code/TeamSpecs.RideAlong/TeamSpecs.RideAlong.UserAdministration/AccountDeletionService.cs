using TeamSpecs.RideAlong.Model;
namespace TeamSpecs.RideAlong.UserAdministration;

public class AccountDeletionService : IAccountDeletionService
{
    private IUserTarget _userTarget;
    public AccountDeletionService(IUserTarget userTarget)
    {
        _userTarget = userTarget;
    }
    public IResponse DeleteUserAccount(string userName)
    {
        #region Validate arguments
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException($"{nameof(userName)} must be valid");
        }
        #endregion

        IResponse response;


        response = _userTarget.DeleteUserAccountSql(userName);

        return response;
    }
}
