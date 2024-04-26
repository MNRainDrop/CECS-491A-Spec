using System.Diagnostics;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class VehicleProfileRetrievalManager : IVehicleProfileRetrievalManager
{
    private readonly ILogService _logService;
    private readonly IVehicleProfileRetrievalService _vpRetrieve;
    private readonly IVehicleProfileDetailsRetrievalService _vpdRetrieve;

    private readonly int numOfResults;
    
    public VehicleProfileRetrievalManager(ILogService logService, IVehicleProfileRetrievalService vpRetrievalService, IVehicleProfileDetailsRetrievalService vpdRetrievalService)
    {
        _logService = logService;
        _vpRetrieve = vpRetrievalService;
        _vpdRetrieve = vpdRetrievalService;
        numOfResults = 10;
    }
    public IResponse GetVehicleProfileDetails(IVehicleProfileModel vehicleProfile, IAccountUserModel userAccount)
    {
        #region Validate Parameters
        if (vehicleProfile == null)
        {
            _logService.CreateLogAsync("Debug", "Data", "Null Vehicle Profile Passed Into Vehicle Profile Retrieval Manager", userAccount.UserHash);
            throw new ArgumentNullException(nameof(vehicleProfile));
        }
        if (string.IsNullOrWhiteSpace(vehicleProfile.VIN))
        {
            _logService.CreateLogAsync("Debug", "Data", "Null VIN in Vehicle Profile Passed Into Vehicle Profile Retrieval Manager", userAccount.UserHash);
            throw new ArgumentNullException(nameof(vehicleProfile.VIN));
        }
        if (vehicleProfile.VIN.Length > 17)
        {
            _logService.CreateLogAsync("Debug", "Data", "Invalid VIN in Vehicle Profile Passed Into Vehicle Profile Retrieval Manager", userAccount.UserHash);
            throw new ArgumentOutOfRangeException(nameof(vehicleProfile.VIN));
        }
        if (string.IsNullOrWhiteSpace(vehicleProfile.LicensePlate))
        {
            _logService.CreateLogAsync("Debug", "Data", "Null VIN in Vehicle Profile Passed Into Vehicle Profile Retrieval Manager", userAccount.UserHash);
            throw new ArgumentNullException(nameof(vehicleProfile.VIN));
        }
        if (vehicleProfile.LicensePlate.Length > 7)
        {
            _logService.CreateLogAsync("Debug", "Data", "Invalid VIN in Vehicle Profile Passed Into Vehicle Profile Retrieval Manager", userAccount.UserHash);
            throw new ArgumentOutOfRangeException(nameof(vehicleProfile.VIN));
        }
        if (userAccount == null)
        {
            _logService.CreateLogAsync("Debug", "Data", "Null User Account Passed Into Vehicle Profile Retrieval Manager", null);
            throw new ArgumentNullException(nameof(userAccount));
        }
        if (userAccount.UserHash is null)
        {
            _logService.CreateLogAsync("Debug", "Data", "Null User Hash Passed Into Vehicle Profile Retrieval Manager", null);
            throw new ArgumentNullException(nameof(userAccount.UserHash));
        }
        #endregion

        #region Call Services
        IResponse response;
        var timer = new Stopwatch();

        timer.Start();
        response = _vpdRetrieve.RetrieveVehicleDetails(vehicleProfile, userAccount);
        timer.Stop();

        if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
        {
            _logService.CreateLogAsync("Warning", "Server", response.ErrorMessage + "Retrieving Vehicle Profiles took longer than 3 seconds, but less than 10.", userAccount.UserHash);
        }
        if (timer.Elapsed.TotalSeconds > 10)
        {
            _logService.CreateLogAsync("Error", "Server", response.ErrorMessage + "Server Timeout on Vehicle Profile Retrieval Service", userAccount.UserHash);
        }
        #endregion

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

    public IResponse GetVehicleProfiles(IAccountUserModel userAccount, int page)
    {
        #region Validate Parameters
        if (userAccount is null)
        {
            _logService.CreateLogAsync("Debug", "Data", "Null User Account Passed Into Vehicle Profile Retrieval Manager", null);
            throw new ArgumentNullException(nameof(userAccount));
        }
        if (userAccount.UserHash is null)
        {
            _logService.CreateLogAsync("Debug", "Data", "Null User Hash Passed Into Vehicle Profile Retrieval Manager", null);
            throw new ArgumentNullException(nameof(userAccount.UserHash));
        }
        #endregion

        #region Call services
        var timer = new Stopwatch();

        timer.Start();
        var response = _vpRetrieve.RetrieveVehicleProfilesForUser(userAccount, numOfResults, page);
        timer.Stop();

        if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
        {
            _logService.CreateLogAsync("Warning", "Server", response.ErrorMessage + "Retrieving Vehicle Profiles took longer than 3 seconds, but less than 10.", userAccount.UserHash);
        }
        if (timer.Elapsed.TotalSeconds > 10)
        {
            _logService.CreateLogAsync("Error", "Server", response.ErrorMessage + "Server Timeout on Vehicle Profile Retrieval Service", userAccount.UserHash);
        }
        #endregion

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
