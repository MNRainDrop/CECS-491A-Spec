using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration.Services;

public class AccountDeletionService : IAccountDeletionService
{
    private readonly IUserTarget _userTarget;
    private readonly ILogService _logService;
    public AccountDeletionService(IUserTarget userTarget, ILogService logService)
    {
        _userTarget = userTarget;
        _logService = logService;
    }
    /// <summary>
    /// Dissocaites all VP's with a account. Deletes any custom values
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public IResponse DeleteVehicles(IAccountUserModel model)
    {
        IResponse response = new Response();

        // implement logic here

        return response;
    }
    /// <summary>
    /// Deletes all tables associated with a user
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public IResponse DeleteUser(IAccountUserModel model)
    {
        IResponse response = new Response();

        // implement logic here

        return response;
    }
}
