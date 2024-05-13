using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;

namespace TeamSpecs.RideAlong.UserAdministration.Services;

public class AccountDeletionService : IAccountDeletionService
{
    private readonly ISqlDbUserDeletionTarget _sqlDbUserDeletion;
    private readonly ILogService _logService;
    public AccountDeletionService(ISqlDbUserDeletionTarget sqlDbUserDeletionTarget, ILogService logService)
    {
        _sqlDbUserDeletion = sqlDbUserDeletionTarget;
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

        response = _sqlDbUserDeletion.DeleteVehicleProfiles(model.UserId);

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

        response = _sqlDbUserDeletion.DeleteUserAccount(model.UserId);

        return response;
    }

    public IResponse CreateAccountDeletionRequestTable(string userHash)
    {
        IResponse response = new Response();

        response = _sqlDbUserDeletion.CreateAccountDeletionRequest(userHash);

        return response;
    }
}
