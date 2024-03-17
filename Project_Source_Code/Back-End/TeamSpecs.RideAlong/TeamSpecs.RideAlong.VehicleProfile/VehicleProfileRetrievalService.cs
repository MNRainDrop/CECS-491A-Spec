using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class VehicleProfileRetrievalService : IVehicleProfileRetrievalService
{
    private readonly IRetrieveVehiclesTarget _vehicleTarget;
    private readonly ILogService _logService;
    public VehicleProfileRetrievalService(IRetrieveVehiclesTarget sqlDbVehicleTarget, ILogService logService)
    {
        _vehicleTarget = sqlDbVehicleTarget;
        _logService = logService;
    }

    public IResponse retrieveVehicleProfilesForUser(IAccountUserModel userAccount)
    {
        #region Validate Parameters
        if (userAccount is null)
        {
            throw new ArgumentNullException(nameof(userAccount));
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
        var response = _vehicleTarget.ReadVehicleProfileSql(search);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve vehicles";
        }
        else
        {
            response.ErrorMessage = "Successful";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
        #endregion
        return response;
    }
 
}
