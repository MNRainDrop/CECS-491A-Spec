using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class VehicleProfileDeletionService : IVehicleProfileDeletionService
{
    private readonly IDeleteVehicleTarget _deleteVehicleTarget;
    private readonly ILogService _logService;
    private readonly IAccountModificationService _accountModificationService;
    public VehicleProfileDeletionService(IDeleteVehicleTarget deleteVehicleTarget, ILogService logService, IAccountModificationService accountModificationService)
    {
        _deleteVehicleTarget = deleteVehicleTarget;
        _logService = logService;
        _accountModificationService = accountModificationService;
    }

    public IResponse deleteVehicleProfile(IVehicleProfileModel vehicle, IAccountUserModel userAccount)
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
            if (userAccount.UserId != vehicle.Owner_UID)
            {
                throw new InvalidDataException(nameof(userAccount.UserId));
            }
        }
        #endregion

        var response = _deleteVehicleTarget.deleteVehicleProfileSql(vehicle, userAccount);

        #region Update Claims
        
        #endregion

        #region Log to database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not delete vehicle. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful deletion of vehicle profile.";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
        #endregion

        return response;
    }
}
