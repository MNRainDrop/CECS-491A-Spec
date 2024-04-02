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
        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            throw new ArgumentNullException(nameof(licensePlate));
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

        var vehicle = new VehicleProfileModel(vin, userAccount.UserId, licensePlate, make, model, year);
        var vehicleDetails = new VehicleDetailsModel(vin, color, description);

        var response = _createVehiclesTarget.CreateVehicleProfileSql(vehicle, vehicleDetails);

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
