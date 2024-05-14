using System.Diagnostics;
using TeamSpecs.RideAlong.CoEsLibrary.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CoEsLibrary.Manager
{
    public class CommEstaManager : ICommEstaManager
    {
        private ISendEmailService _sendEmailService;
        private readonly ILogService _logService;

        public CommEstaManager(ISendEmailService sendEmailService, ILogService logService)
        {
            _sendEmailService = sendEmailService;
            _logService = logService;
        }

        public IResponse GetSeller(IAccountUserModel model, string vin)
        {

            IResponse response = new Model.Response();
            var timer = new Stopwatch();
            try
            {
                timer.Start();
                response = _sendEmailService.SendEmail(model, vin);
                timer.Stop();
            }
            catch (Exception ex)
            {
                response.ErrorMessage += ex.Message;
                response.HasError = true;
            }

            if (timer.Elapsed.TotalSeconds > 3 && timer.Elapsed.TotalSeconds <= 10)
            {
                _logService.CreateLogAsync("Warning", "Server", "Sending email to User took longer than 3 seconds, but less than 10. " + response.ErrorMessage, model.UserHash);
            }
            if (timer.Elapsed.TotalSeconds > 10)
            {
                _logService.CreateLogAsync("Error", "Server", "Server Timeout on Sending Email Service. " + response.ErrorMessage, model.UserHash);
            }

            if (response.HasError)
            {
                response.ErrorMessage = "Could not send eamil to account. " + response.ErrorMessage;
            }
            else
            {
                response.ErrorMessage = "Successfully sent email account. " + response.ErrorMessage;
            }
            _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, model.UserHash);
            return response;
        }
    }
}
