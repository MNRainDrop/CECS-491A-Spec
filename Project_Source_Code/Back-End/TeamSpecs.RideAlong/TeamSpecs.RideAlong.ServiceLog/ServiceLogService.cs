using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model.ServiceLogModel;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.ServiceLog.Interfaces;
using Microsoft.Data.SqlClient;

namespace TeamSpecs.RideAlong.ServiceLog
{
    public class ServiceLogService : IServiceLogService
    {
        private readonly ISqlDbServiceLogTarget _serviceLogTarget;
        private readonly ILogService _logService;

        ServiceLogService(ILogService logService, ISqlDbServiceLogTarget serviceLogTarget) 
        {
            _logService = logService;
            _serviceLogTarget = serviceLogTarget;
        }

        public IResponse CreateServiceLog(IServiceLogModel serviceLog, IAccountUserModel userAccount)
        {
            #region Variables
            IResponse response = new Response();
            #endregion

            #region Validate Parameters
            if (userAccount is null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }
            if (string.IsNullOrWhiteSpace(userAccount.UserHash))
            {
                throw new ArgumentNullException(nameof(userAccount.UserHash));
            }
            if (serviceLog is null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }
            if (string.IsNullOrWhiteSpace(serviceLog.Category))
            {
                throw new ArgumentNullException(nameof(serviceLog.Category));
            }
            if (string.IsNullOrWhiteSpace(serviceLog.Part))
            {
                throw new ArgumentNullException(nameof(serviceLog.Part));
            }
            if(string.IsNullOrWhiteSpace(serviceLog.Description))
            {
                throw new ArgumentNullException(nameof(serviceLog.Description));
            }
            if (string.IsNullOrWhiteSpace(serviceLog.VIN))
            {
                throw new ArgumentNullException(nameof(serviceLog.VIN));
            }
            // Add DateTime null check
            #endregion

            response = _serviceLogTarget.GenerateCreateServiceLogSql(serviceLog);

            #region Logging to Database
            if (response.HasError)
            {
                response.ErrorMessage = "Could not create Service Log. " + response.ErrorMessage;
            }
            else
            {
                response.ErrorMessage = "Successful retrieval of Service Log.";
            }
            _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
            response.ErrorMessage = "";
            #endregion

            return response;
        }

        public IResponse ModifyServiceLog(IServiceLogModel serviceLog, IAccountUserModel userAccount)
        {
            #region Variables
            IResponse response = new Response();
            #endregion

            throw new NotImplementedException();
        }

        public IResponse DeleteServiceLog(IAccountUserModel userAccount)
        {
            #region Variables
            IResponse response = new Response();
            #endregion

            throw new NotImplementedException();
        }

        public IResponse RetrieveServiceLogs(IAccountUserModel userAccount)
        {
            #region Variables
            IResponse response = new Response();
            #endregion

            throw new NotImplementedException();
        }

        public IResponse RetrieveServiceLogDetails(IAccountUserModel userAccount)
        {
            #region Variables
            IResponse response = new Response();
            #endregion

            throw new NotImplementedException();
        }

        public IResponse RetrieveFilteredServiceLogs(IAccountUserModel userAccount)
        {
            #region Variables
            IResponse response = new Response();
            #endregion

            throw new NotImplementedException();
        }

        public IResponse CreateMantainenceReminder(IAccountUserModel userAccount)
        {
            #region Variables
            IResponse response = new Response();
            #endregion

            throw new NotImplementedException();
        }
    }
}
