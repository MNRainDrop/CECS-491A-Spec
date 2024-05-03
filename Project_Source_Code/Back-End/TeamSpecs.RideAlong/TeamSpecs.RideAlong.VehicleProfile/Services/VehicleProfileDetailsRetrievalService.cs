using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class VehicleProfileDetailsRetrievalService : IVehicleProfileDetailsRetrievalService
{
    private readonly ICRUDVehicleTarget _vehicleDetailsTarget;
    private readonly ILogService _logService;

    public VehicleProfileDetailsRetrievalService(ICRUDVehicleTarget vehicleDetailsTarget, ILogService log)
    {
        _vehicleDetailsTarget = vehicleDetailsTarget;
        _logService = log;
    }

    public IResponse RetrieveVehicleDetails(IVehicleProfileModel vehicleProfile, IAccountUserModel userAccount)
    {
        #region Check Parameters
        if (vehicleProfile is null)
        {
            throw new ArgumentNullException(nameof(vehicleProfile));
        }
        if (string.IsNullOrWhiteSpace(vehicleProfile.VIN))
        {
            throw new ArgumentNullException(nameof(vehicleProfile.VIN));
        }
        if (vehicleProfile.VIN.Length > 17)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicleProfile.VIN));
        }
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
            new KeyValuePair<string, string>("VIN", vehicleProfile.VIN)
        };
        var response = _vehicleDetailsTarget.ReadVehicleProfileDetailsSql(search);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve vehicle profile details." + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of vehicle profile details.";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
        #endregion

        return response;
    }
}
