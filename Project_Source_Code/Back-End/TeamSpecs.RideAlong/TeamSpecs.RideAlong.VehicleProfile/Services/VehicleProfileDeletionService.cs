using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class VehicleProfileDeletionService : IVehicleProfileDeletionService
{
    private readonly ICRUDVehicleTarget _deleteVehicleTarget;
    private readonly ILogService _logService;
    private readonly IClaimService _claimService;
    public VehicleProfileDeletionService(ICRUDVehicleTarget deleteVehicleTarget, ILogService logService, IClaimService claimService)
    {
        _deleteVehicleTarget = deleteVehicleTarget;
        _logService = logService;
        _claimService = claimService;
    }

    public IResponse DeleteVehicleProfile(IVehicleProfileModel vehicle, IAccountUserModel userAccount)
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
        if (userAccount is null)
        {
            throw new ArgumentNullException(nameof(userAccount));
        }
        if (string.IsNullOrWhiteSpace(userAccount.UserHash))
        {
            throw new ArgumentNullException(nameof(userAccount.UserHash));
        }
        if (string.IsNullOrWhiteSpace(userAccount.UserName))
        {
            throw new ArgumentNullException(nameof(userAccount.UserName));
        }
        if (userAccount.UserId != vehicle.Owner_UID)
        {
            throw new InvalidDataException(nameof(userAccount.UserId));
        }
        #endregion

        var response = _deleteVehicleTarget.DeleteVehicleProfileSql(vehicle, userAccount);

        #region Error Check and Update Claims
        if (response.HasError)
        {
            response.ErrorMessage = "Could not delete vehicle. " + response.ErrorMessage;
        }
        else
        {
            // Remove claims
            _claimService.DeleteUserClaim(userAccount, "canViewVehicle", vehicle.VIN);
            _claimService.DeleteUserClaim(userAccount, "canModifyVehicle", vehicle.VIN);
            _claimService.DeleteUserClaim(userAccount, "canDeleteVehicle", vehicle.VIN);

            response.ErrorMessage = "Successful deletion of vehicle profile.";
        }
        #endregion

        #region Log to database
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
        #endregion

        return response;
    }
}
