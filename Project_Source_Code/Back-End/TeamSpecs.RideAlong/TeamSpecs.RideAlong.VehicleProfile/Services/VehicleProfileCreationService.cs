namespace TeamSpecs.RideAlong.VehicleProfile;

using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

public class VehicleProfileCreationService : IVehicleProfileCreationService
{
    private readonly ICreateVehicleTarget _createVehiclesTarget;
    private readonly ILogService _logService;

    public VehicleProfileCreationService(ILogService logService, ICreateVehicleTarget createVehiclesTarget)
    {
        _logService = logService;
        _createVehiclesTarget = createVehiclesTarget;
    }
    public IResponse createVehicleProfile(string vin, string licensePlate, string? make, string? model, int year, string? color, string? description, IAccountUserModel userAccount)
    {
        #region Validate Parameters
        if (string.IsNullOrWhiteSpace(vin))
        {
            throw new ArgumentNullException(nameof(vin));
        }
        if (vin.Length > 17)
        {
            throw new ArgumentOutOfRangeException(nameof(vin));
        }
        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            throw new ArgumentNullException(nameof(licensePlate));
        }
        if (licensePlate.Length > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(licensePlate));
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

        var vehicle = new VehicleProfileModel(vin, userAccount.UserId, licensePlate, (!string.IsNullOrWhiteSpace(make)) ? make : "", (!string.IsNullOrWhiteSpace(model)) ? model : "", year);
        var vehicleDetails = new VehicleDetailsModel(vin, (!string.IsNullOrWhiteSpace(color)) ? color : "", (!string.IsNullOrWhiteSpace(description)) ? description : "");

        return createVehicleProfile(vehicle, vehicleDetails, userAccount);
    }

    public IResponse createVehicleProfile(IVehicleProfileModel vehicle, IVehicleDetailsModel vehicleDetails, IAccountUserModel userAccount)
    {
        #region Validate Parameters
        if (string.IsNullOrWhiteSpace(vehicle.VIN))
        {
            if (string.IsNullOrWhiteSpace(vehicleDetails.VIN))
            {
                throw new ArgumentNullException(nameof(vehicle.VIN));
            }
            else
            {
                vehicle.VIN = vehicleDetails.VIN;
            }
        }
        if (string.IsNullOrWhiteSpace(vehicle.LicensePlate))
        {
            throw new ArgumentNullException(nameof(vehicle.LicensePlate));
        }
        if (string.IsNullOrWhiteSpace(vehicleDetails.VIN))
        {
            if (string.IsNullOrWhiteSpace(vehicle.VIN))
            {
                throw new ArgumentNullException(nameof(vehicleDetails.VIN));
            }
            else
            {
                vehicleDetails.VIN = vehicle.VIN;
            }
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
        var response = _createVehiclesTarget.createVehicleProfileSql(vehicle, vehicleDetails);

        #region Update Claims
        // add claims here once user administration claim modification is complete
        #endregion

        #region Log to database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not create vehicle. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of vehicle profile.";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
        #endregion

        return response;
    }
}
