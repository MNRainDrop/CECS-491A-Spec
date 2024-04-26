namespace TeamSpecs.RideAlong.VehicleProfile;

using System.Linq;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

public class VehicleProfileCreationService : IVehicleProfileCreationService
{
    private readonly ICRUDVehicleTarget _crudVehiclesTarget;
    private readonly ILogService _logService;

    public VehicleProfileCreationService(ILogService logService, ICRUDVehicleTarget crudVehiclesTarget)
    {
        _logService = logService;
        _crudVehiclesTarget = crudVehiclesTarget;
    }
    public IResponse CreateVehicleProfile(string vin, string licensePlate, string? make, string? model, int year, string? color, string? description, IAccountUserModel userAccount)
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

        return CreateVehicleProfile(vehicle, vehicleDetails, userAccount);
    }

    public IResponse CreateVehicleProfile(IVehicleProfileModel vehicle, IVehicleDetailsModel vehicleDetails, IAccountUserModel userAccount)
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

        #region Write vehicle to database
        // Check if vehicle is in database
        // if vehicle is in the database, change the owner to the new owner
        // if vehicle is not in the database, write a new entry for the vehicle
        var search = new List<object>()
        {
            new KeyValuePair<string, string>("VIN", vehicle.VIN)
        };
        var vehicleInDB = _crudVehiclesTarget.ReadVehicleProfileSql(search, 1, 1);

        IResponse response;
        if (vehicleInDB.ReturnValue is not null)
        {
            if (vehicleInDB.ReturnValue.Count == 0)
            {
                response = _crudVehiclesTarget.CreateVehicleProfileSql(vehicle, vehicleDetails);
            }
            else
            {
                var tempVehicle = vehicleInDB.ReturnValue.First() as IVehicleProfileModel;
                if (tempVehicle is not null && tempVehicle.Owner_UID is null)
                {
                    response = _crudVehiclesTarget.UpdateVehicleOwnerSql(vehicle, vehicleDetails);
                }
                else
                {
                    response = new Response();
                    response.HasError = true;
                    response.ErrorMessage = $"{vehicle.VIN} is already in database. ";
                }
            }
        }
        else
        {
            response = new Response();
            response.HasError = true;
            response.ErrorMessage = $"Could not detect if {vehicle.VIN} is in database. ";
            return response;
        }
        #endregion

        #region Error Check
        if (response.HasError)
        {
            response.ErrorMessage = "Could not create vehicle. " + response.ErrorMessage;
        }
        else
        {
            // Update Claims

            // Add claims here once user administration claim modification is complete
            var claims = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("canViewVehicle", vehicle.VIN),
                new KeyValuePair<string, string>("canModifyVehicle", vehicle.VIN),
                new KeyValuePair<string, string>("canDeleteVehicle", vehicle.VIN),
                new KeyValuePair<string, string>("ownsVehicle", "true")
            };

            // add claims

            response.ErrorMessage = "Successful retrieval of vehicle profile.";
        }
        #endregion

        #region Log action to database

        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
        #endregion

        return response;
    }
}
