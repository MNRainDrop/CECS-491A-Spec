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
        throw new NotImplementedException();
    }
}
