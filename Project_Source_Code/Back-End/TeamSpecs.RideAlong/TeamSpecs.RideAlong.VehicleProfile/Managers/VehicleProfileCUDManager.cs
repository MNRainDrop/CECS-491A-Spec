using System.Diagnostics;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class VehicleProfileCUDManager : ICreateVehicleProfileManager, IModifyVehicleProfileManager, IDeleteVehicleProfileManager
{
    private readonly ILogService _logService;
    private readonly IVehicleProfileCreationService _vpCreate;
    private readonly IVehicleProfileModificationService _vpModify;
    private readonly IVehicleProfileDeletionService _vpDelete;

    public VehicleProfileCUDManager(ILogService logService, IVehicleProfileCreationService vpCreate, IVehicleProfileModificationService vpModify, IVehicleProfileDeletionService vpDelete)
    {
        _logService = logService;
        _vpCreate = vpCreate;
        _vpModify = vpModify;
        _vpDelete = vpDelete;
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
            response = _vpCreate.createVehicleProfile(vehicle, vehicleDetails, account);
            timer.Stop();
        }
        catch (Exception ex)
        {
            response.ErrorMessage += ex.Message;
            response.HasError = true;
        }

        if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
        {
            _logService.CreateLogAsync("Warning", "Server", "Creating Vehicle Profile took longer than 3 seconds, but less than 10. " + response.ErrorMessage, account.UserHash);
        }
        if (timer.Elapsed.TotalSeconds > 10)
        {
            _logService.CreateLogAsync("Error", "Server", "Server Timeout on Vehicle Profile Creation Service. " + response.ErrorMessage, account.UserHash);
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
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, account.UserHash);
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
            _logService.CreateLogAsync("Warning", "Server", "Creating Vehicle Profile took longer than 3 seconds, but less than 10. " + response.ErrorMessage, account.UserHash);
        }
        if (timer.Elapsed.TotalSeconds > 10)
        {
            _logService.CreateLogAsync("Error", "Server", "Server Timeout on Vehicle Profile Creation Service. " + response.ErrorMessage, account.UserHash);
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
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, account.UserHash);
        #endregion
        return response;
    }

    public IResponse DeleteVehicleProfile()
    {
        throw new NotImplementedException();
    }
}
