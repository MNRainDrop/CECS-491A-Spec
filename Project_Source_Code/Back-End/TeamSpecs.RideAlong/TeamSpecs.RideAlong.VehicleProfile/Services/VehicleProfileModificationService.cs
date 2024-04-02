namespace TeamSpecs.RideAlong.VehicleProfile;

using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

public class VehicleProfileModificationService : IVehicleProfileModificationService
{
    private readonly IModifyVehicleTarget _modifyVehicleTarget;
    private readonly ILogService _logService;

    public VehicleProfileModificationService(IModifyVehicleTarget vehicleTarget, ILogService logService)
    {
        _modifyVehicleTarget = vehicleTarget;
        _logService = logService;
    }

    public IResponse ModifyVehicleProfile(IVehicleProfileModel vehicle, IVehicleDetailsModel vehicleDetails, IAccountUserModel userAccount)
    {
        #region Validate Parameters
        if (vehicle is null)
        {
            throw new ArgumentNullException(nameof(vehicle));
        }
        if (string.IsNullOrWhiteSpace(vehicle.VIN))
        {
            throw new ArgumentNullException(nameof(vehicle.VIN));
        }
        if (string.IsNullOrWhiteSpace(vehicle.LicensePlate))
        {
            throw new ArgumentNullException(nameof(vehicle.LicensePlate));
        }
        if (vehicleDetails is null)
        {
            throw new ArgumentNullException(nameof(vehicleDetails));
        }
        if (string.IsNullOrWhiteSpace(vehicleDetails.VIN))
        {
            throw new ArgumentNullException(nameof(vehicleDetails.VIN));
        }
        // Make can be null
        // Model can be null
        // Color can be null
        // Description can be null
        if (userAccount is null)
        {
            throw new ArgumentNullException(nameof(userAccount));
        }
        else
        {
            if (string.IsNullOrWhiteSpace(userAccount.UserHash))
            {
                throw new ArgumentNullException(nameof(userAccount.UserHash));
            }
            if (string.IsNullOrWhiteSpace(userAccount.UserName))
            {
                throw new ArgumentNullException(nameof(userAccount.UserName));
            }
        }
        #endregion

        var response = _modifyVehicleTarget.ModifyVehicleProfileSql(vehicle, vehicleDetails);

        #region Log to database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not create vehicle. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful modification of vehicle profile.";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
        #endregion

        return response;
    }
}
