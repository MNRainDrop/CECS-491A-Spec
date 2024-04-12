using Azure;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TeamSpecs.RideAlong.CarNewsCenter
{
    public class ViewVehicleNewsArticleService : IViewVehicleNewsArticleServices
    {
        private readonly ICarNewsCenterTarget _vehicleTarget;
        private readonly ILogService _logService;
        public ViewVehicleNewsArticleService(ICarNewsCenterTarget sqlDbCarNewsCenterTarget, ILogService logService)
        {
            _vehicleTarget = sqlDbCarNewsCenterTarget;
            _logService = logService;
        }
        public IResponse GetNewsForAllVehicles(IAccountUserModel userAccount)
        {
            #region Validate Parameters
            if (userAccount is null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }
            if (string.IsNullOrWhiteSpace(userAccount.UserHash))
            {
                throw new ArgumentNullException(nameof(userAccount.UserHash));
            }
            #endregion

            var search = new List<object>()
        {
            new KeyValuePair<string, long>("Owner_UID", userAccount.UserId)
        };
            var response = _vehicleTarget.GetsAllVehicles(search);

            #region Log the action to the database
            if (response.HasError)
            {
                response.ErrorMessage = "Could not retrieve vehicles. " + response.ErrorMessage;
            }
            else
            {
                response.ErrorMessage = "Successful retrieval of vehicle profile. ";
            }
            _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
            #endregion
            return response;
        }
    }
}
