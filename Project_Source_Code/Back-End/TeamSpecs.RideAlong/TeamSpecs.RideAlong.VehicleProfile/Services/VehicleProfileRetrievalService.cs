using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class VehicleProfileRetrievalService : IVehicleProfileRetrievalService
{
    private readonly ICRUDVehicleTarget _vehicleTarget;
    private readonly ILogService _logService;
    public VehicleProfileRetrievalService(ICRUDVehicleTarget sqlDbVehicleTarget, ILogService logService)
    {
        _vehicleTarget = sqlDbVehicleTarget;
        _logService = logService;
    }

    public IResponse RetrieveVehicleProfilesForUser(IAccountUserModel userAccount, int numOfResults, int page)
    {
        #region Validate Parameters
        if (userAccount is null)
        {
            throw new ArgumentNullException(nameof(userAccount));
        }
        if (string.IsNullOrEmpty(userAccount.UserName))
        {
            throw new ArgumentNullException(nameof(userAccount.UserName));
        }
        if (string.IsNullOrWhiteSpace(userAccount.UserHash))
        {
            throw new ArgumentNullException(nameof(userAccount.UserHash));
        }
        #endregion

        var search = new List<object>()
        {
            new KeyValuePair<string, long>("Owner_UID", userAccount.UserId)
        };
        var response = _vehicleTarget.ReadVehicleProfileSql(search, numOfResults, page);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve vehicles. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of vehicle profile. ";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
        #endregion
        return response;
    }
}
