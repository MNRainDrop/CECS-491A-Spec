using System.Diagnostics;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class VehicleProfileCUDManager : IVehicleProfileCUDManager
{
    private readonly ILogService _logService;
    private readonly IVehicleProfileCreationService _vpCreate;
    private readonly IVehicleProfileModificationService _vpModify;
    private readonly IVehicleProfileDeletionService _vpDelete;
    private readonly IGetVehicleCountTarget _vpCount;
    private readonly IConfigServiceJson _config;
    private readonly IClaimService _claimService;

    private readonly int _maxOwnedCars;

    public VehicleProfileCUDManager(
        ILogService logService,
        IVehicleProfileCreationService vpCreate,
        IVehicleProfileModificationService vpModify,
        IVehicleProfileDeletionService vpDelete,
        IGetVehicleCountTarget vpCount,
        IConfigServiceJson configService,
        IClaimService claimService)
    {
        _logService = logService;
        _vpCreate = vpCreate;
        _vpModify = vpModify;
        _vpDelete = vpDelete;
        _vpCount = vpCount;
        _config = configService;
        _claimService = claimService;

        _maxOwnedCars = _config.GetConfig().VEHICLE_PROFILE_MANAGER.MAXOWNEDCARS;
    }

    public IResponse CreateVehicleProfile(IVehicleProfileModel vehicle, IVehicleDetailsModel vehicleDetails, IAccountUserModel account)
    {
        #region Validate Parameters
        // Vehicle Profile
        if (vehicle is null)
        {
            throw new ArgumentNullException(nameof(vehicle));
        }
        if (string.IsNullOrWhiteSpace(vehicle.VIN))
        {
            throw new ArgumentNullException(nameof(vehicle.VIN));
        }
        if (vehicle.VIN.Length > 17)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicle.VIN));
        }
        if (string.IsNullOrWhiteSpace(vehicle.LicensePlate))
        {
            throw new ArgumentNullException(nameof(vehicle.LicensePlate));
        }
        if (vehicle.LicensePlate.Length > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicle.LicensePlate));
        }
        if (string.IsNullOrEmpty(vehicle.Make))
        {
            throw new ArgumentNullException(nameof(vehicle.Make));
        }
        if (string.IsNullOrEmpty(vehicle.Model))
        {
            throw new ArgumentNullException(nameof(vehicle.Model));
        }
        if (vehicle.Year < 1990)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicle.Year));
        }

        // Vehicle Details
        if (vehicleDetails is null)
        {
            throw new ArgumentNullException(nameof(vehicleDetails));
        }
        if (string.IsNullOrEmpty(vehicleDetails.VIN))
        {
            vehicleDetails.VIN = vehicle.VIN;
        }
        if (vehicleDetails.Color.Length > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicleDetails.Color));
        }
        if (vehicleDetails.Description.Length > 500)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicleDetails.Description));
        }

        // User Account
        if (account is null)
        {
            throw new ArgumentNullException(nameof(account));
        }
        if (string.IsNullOrWhiteSpace(account.UserName))
        {
            throw new ArgumentNullException(nameof(account.UserName));
        }
        if (string.IsNullOrWhiteSpace(account.UserHash))
        {
            throw new ArgumentNullException(nameof(account.UserHash));
        }
        #endregion

        #region Call Services
        IResponse response = new Response();
        var timer = new Stopwatch();

        try
        {
            timer.Start();
            response = _vpCreate.CreateVehicleProfile(vehicle, vehicleDetails, account);
            timer.Stop();
        }
        catch (Exception ex)
        {
            response.ErrorMessage += ex.Message;
            response.HasError = true;
        }

        if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
        {
            _logService.CreateLogAsync("Warning", "Business", "Creating Vehicle Profile took longer than 3 seconds, but less than 10. " + response.ErrorMessage, account.UserHash);
        }
        if (timer.Elapsed.TotalSeconds > 10)
        {
            _logService.CreateLogAsync("Error", "Business", "Server Timeout on Vehicle Profile Creation Service. " + response.ErrorMessage, account.UserHash);
        }

        // Updating can create vehicle claim
        var vehicleCount = _vpCount.GetVehicleCount(account);
        if (vehicleCount.ReturnValue is not null)
        {
            var value = vehicleCount.ReturnValue.First<object>() as object[];
            if (value != null && (int)value[0] >= _maxOwnedCars - 1)
            {
                var oldClaim = new KeyValuePair<string, string>("canCreateVehicle", "true");
                var newClaim = new KeyValuePair<string, string>("canCreateVehicle", "false");

                _claimService.ModifyUserClaim(account, oldClaim, newClaim);
            }
        }
        #endregion

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not create vehicle profile. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of vehicle profile details. " + response.ErrorMessage;
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Business", response.ErrorMessage, account.UserHash);
        #endregion
        return response;
    }

    public IResponse ModifyVehicleProfile(IVehicleProfileModel vehicle, IVehicleDetailsModel vehicleDetails, IAccountUserModel account)
    {
        #region Validate Parameters
        // Vehicle Profile
        if (vehicle is null)
        {
            throw new ArgumentNullException(nameof(vehicle));
        }
        if (string.IsNullOrWhiteSpace(vehicle.VIN))
        {
            throw new ArgumentNullException(nameof(vehicle.VIN));
        }
        if (vehicle.VIN.Length > 17)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicle.VIN));
        }
        // License plate can be null or whitespace if the car is not registered.
        if (string.IsNullOrWhiteSpace(vehicle.LicensePlate))
        {
            vehicle.LicensePlate = "";
        }
        if (vehicle.LicensePlate.Length > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicle.LicensePlate));
        }
        if (string.IsNullOrEmpty(vehicle.Make))
        {
            throw new ArgumentNullException(nameof(vehicle.Make));
        }
        if (string.IsNullOrEmpty(vehicle.Model))
        {
            throw new ArgumentNullException(nameof(vehicle.Model));
        }
        if (vehicle.Year < 1990)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicle.Year));
        }

        // Vehicle Details
        if (vehicleDetails is null)
        {
            throw new ArgumentNullException(nameof(vehicleDetails));
        }
        if (string.IsNullOrEmpty(vehicleDetails.VIN) || !vehicleDetails.VIN.Equals(vehicle.VIN))
        {
            throw new ArgumentNullException(nameof(vehicleDetails.VIN));
        }
        if (vehicleDetails.Color.Length > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicleDetails.Color));
        }
        if (vehicleDetails.Description.Length > 500)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicleDetails.Description));
        }

        // User Account
        if (account is null)
        {
            throw new ArgumentNullException(nameof(account));
        }
        if (string.IsNullOrWhiteSpace(account.UserName))
        {
            throw new ArgumentNullException(nameof(account.UserName));
        }
        if (string.IsNullOrWhiteSpace(account.UserHash))
        {
            throw new ArgumentNullException(nameof(account.UserHash));
        }
        #endregion

        #region Call Services
        IResponse response = new Response();
        var timer = new Stopwatch();

        try
        {
            timer.Start();
            response = _vpModify.ModifyVehicleProfile(vehicle, vehicleDetails, account);
            timer.Stop();
        }
        catch (Exception ex)
        {
            response.ErrorMessage += ex.Message;
            response.HasError = true;
        }

        if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
        {
            _logService.CreateLogAsync("Warning", "Server", "Modifying Vehicle Profile took longer than 3 seconds, but less than 10. " + response.ErrorMessage, account.UserHash);
        }
        if (timer.Elapsed.TotalSeconds > 10)
        {
            _logService.CreateLogAsync("Error", "Server", "Server Timeout on Vehicle Profile Modification Service. " + response.ErrorMessage, account.UserHash);
        }
        #endregion

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not modify vehicle profile. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful modification of vehicle profile and details. " + response.ErrorMessage;
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Business", response.ErrorMessage, account.UserHash);
        #endregion
        return response;
    }

    public IResponse DeleteVehicleProfile(IVehicleProfileModel vehicle, IAccountUserModel account)
    {
        #region Validate Parameters
        // Vehicle Profile
        if (vehicle is null)
        {
            throw new ArgumentNullException(nameof(vehicle));
        }
        if (string.IsNullOrWhiteSpace(vehicle.VIN))
        {
            throw new ArgumentNullException(nameof(vehicle.VIN));
        }
        if (vehicle.VIN.Length > 17)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicle.VIN));
        }
        // License plate can be null or whitespace if the car is not registered.
        if (string.IsNullOrWhiteSpace(vehicle.LicensePlate))
        {
            vehicle.LicensePlate = "";
        }
        if (vehicle.LicensePlate.Length > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicle.LicensePlate));
        }
        if (string.IsNullOrEmpty(vehicle.Make))
        {
            throw new ArgumentNullException(nameof(vehicle.Make));
        }
        if (string.IsNullOrEmpty(vehicle.Model))
        {
            throw new ArgumentNullException(nameof(vehicle.Model));
        }
        if (vehicle.Year < 1990)
        {
            throw new ArgumentOutOfRangeException(nameof(vehicle.Year));
        }

        // User Account
        if (account is null)
        {
            throw new ArgumentNullException(nameof(account));
        }
        if (string.IsNullOrWhiteSpace(account.UserName))
        {
            throw new ArgumentNullException(nameof(account.UserName));
        }
        if (string.IsNullOrWhiteSpace(account.UserHash))
        {
            throw new ArgumentNullException(nameof(account.UserHash));
        }
        #endregion

        #region Call Services
        IResponse response = new Response();
        var timer = new Stopwatch();

        try
        {
            timer.Start();
            response = _vpDelete.DeleteVehicleProfile(vehicle, account);
            timer.Stop();
        }
        catch (Exception ex)
        {
            response.ErrorMessage += ex.Message;
            response.HasError = true;
        }

        // Updating can create vehicle claim
        var vehicleCount = _vpCount.GetVehicleCount(account);
        if (vehicleCount.ReturnValue is not null)
        {
            var value = vehicleCount.ReturnValue.First<object>() as object[];
            if (value != null && (int)value[0] <= _maxOwnedCars - 1)
            {
                var oldClaim = new KeyValuePair<string, string>("canCreateVehicle", "false");
                var newClaim = new KeyValuePair<string, string>("canCreateVehicle", "true");

                _claimService.ModifyUserClaim(account, oldClaim, newClaim);
            }
        }

        if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
        {
            _logService.CreateLogAsync("Warning", "Business", "Deleting Vehicle Profile took longer than 3 seconds, but less than 10. " + response.ErrorMessage, account.UserHash);
        }
        if (timer.Elapsed.TotalSeconds > 10)
        {
            _logService.CreateLogAsync("Error", "Business", "Server Timeout on Vehicle Profile Deletion Service. " + response.ErrorMessage, account.UserHash);
        }
        #endregion

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not delete vehicle profile. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful deletion vehicle profile. " + response.ErrorMessage;
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Business", response.ErrorMessage, account.UserHash);
        #endregion
        return response;
    }
}
