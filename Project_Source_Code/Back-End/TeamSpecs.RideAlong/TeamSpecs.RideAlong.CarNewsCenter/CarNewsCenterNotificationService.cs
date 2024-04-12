using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.CarNewsCenter.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CarNewsCenter
{
    public class CarNewsCenterNotificationService : ICarNewsCenterNotificationService
    {
        private readonly ICarNewsCenterTarget _vehicleTarget;
        private readonly ILogService _logService;
        public CarNewsCenterNotificationService(ICarNewsCenterTarget sqlDbCarNewsCenterTarget, ILogService logService)
        {
            _vehicleTarget = sqlDbCarNewsCenterTarget;
            _logService = logService;
        }

        public IResponse NotificationAlert(INotification notification)
        {
            #region Validate Parameters
            if (notification is null)
            {
                throw new ArgumentNullException(nameof(notification));
            }
            if (string.IsNullOrWhiteSpace(notification.type))
            {
                throw new ArgumentNullException(nameof(notification.type));
            }
            if (string.IsNullOrWhiteSpace(notification.description))
            {
                throw new ArgumentNullException(nameof(notification.description));
            }
            #endregion

            //calling target
            var response = _vehicleTarget.UpdateNotification(notification);


            #region Log the action to the database
            if (response.HasError)
            {
                response.ErrorMessage = "Could not retrieve vehicles. " + response.ErrorMessage;
            }
            else
            {
                response.ErrorMessage = "Successful retrieval of vehicle profile. ";
            }
            _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage,null);
            #endregion
            return response;

        }
    }
}
