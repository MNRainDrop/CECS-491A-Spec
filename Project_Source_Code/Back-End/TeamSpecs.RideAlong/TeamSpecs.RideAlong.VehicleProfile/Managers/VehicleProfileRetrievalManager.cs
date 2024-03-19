using Azure;
using System.Diagnostics;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleProfile;

public class VehicleProfileRetrievalManager : IGetVehicleProfilesManager, IGetVehicleProfileDetailsManager
{
    private readonly ILogService _logService;
    private readonly IVehicleProfileRetrievalService _vpRetrieve;
    private readonly IVehicleProfileDetailsRetrievalService _vpdRetrieve;
    
    public VehicleProfileRetrievalManager(ILogService logService, IVehicleProfileRetrievalService vpRetrievalService, IVehicleProfileDetailsRetrievalService vpdRetrievalService)
    {
        _logService = logService;
        _vpRetrieve = vpRetrievalService;
        _vpdRetrieve = vpdRetrievalService;
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
        if (vehicleProfile.VIN.Length != 17)
        {
            _logService.CreateLogAsync("Debug", "Data", "Invalid VIN in Vehicle Profile Passed Into Vehicle Profile Retrieval Manager", userAccount.UserHash);
            throw new ArgumentOutOfRangeException(nameof(vehicleProfile.VIN));
        }
        if (userAccount == null)
        {
            _logService.CreateLogAsync("Debug", "Data", "Null Vehicle Profile Passed Into Vehicle Profile Retrieval Manager", null);
            throw new ArgumentNullException(nameof(userAccount));
        }
        if (userAccount.UserHash is null)
        {
            _logService.CreateLogAsync("Debug", "Data", "Null Vehicle Profile Passed Into Vehicle Profile Retrieval Manager", null);
            throw new ArgumentNullException(nameof(userAccount));
        }
        #endregion
        IResponse response;

        #region Call Services
        response = _vpdRetrieve.retrieveVehicleDetails(vehicleProfile, userAccount);
        #endregion

        #region Log the action to the database

        #endregion
        return response;
    }

    public IResponse GetVehicleProfiles(IAccountUserModel userAccount)
    {
        #region Validate Parameters
        if (userAccount is null)
        {
            _logService.CreateLogAsync("Debug", "Data", "Null Vehicle Profile Passed Into Vehicle Profile Retrieval Manager", null);
            throw new ArgumentNullException(nameof(userAccount));
        }
        if (userAccount.UserHash is null)
        {
            _logService.CreateLogAsync("Debug", "Data", "Null Vehicle Profile Passed Into Vehicle Profile Retrieval Manager", null);
            throw new ArgumentNullException(nameof(userAccount));
        }
        #endregion

        IResponse response;



        #region Call services
        var timer = new Stopwatch();

        timer.Start();
        response = _vpRetrieve.retrieveVehicleProfilesForUser(userAccount);
        timer.Stop();

        if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
        {
            _logService.CreateLogAsync("Warning", "Server", response.ErrorMessage + "Retrieving Vehicle Profiles took longer than 3 seconds, but less than 10.", userAccount.UserHash);
        }
        if (timer.Elapsed.TotalSeconds > 10)
        {
            _logService.CreateLogAsync("Error", "Server", response.ErrorMessage + "Server Timeout", userAccount.UserHash);
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
