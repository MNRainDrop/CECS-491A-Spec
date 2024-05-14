using TeamSpecs.RideAlong.CoEsLibrary.Interfaces;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.CoEsLibrary.Services
{
    public class SendEmailService : ISendEmailService
    {
        private readonly ILogService _logService;
        private readonly ISqlDbCETarget _dbCETarget;
        private readonly IMailKitService _mailKitService;

        public SendEmailService(ILogService logService, ISqlDbCETarget dbCETarget, IMailKitService mailKitService)
        {
            _logService = logService;
            _dbCETarget = dbCETarget;
            _mailKitService = mailKitService;

        }

        public IResponse SendEmail(IAccountUserModel accountUser, string vin)
        {
            IResponse response = new Response();
            response = _dbCETarget.GetSellerSql(vin);
            var username = "";


            if (response.ReturnValue is not null)
            {
                if (response.ReturnValue.ToList()[0] is object[] array)
                {
                    username = array[0].ToString();

                    string emailBody = $@"

                        Dear {username},

                        The user {accountUser.UserName} wishes to communicate with you about your vehicle post with vin number: {vin} 

                        If you wish to accept his request use his email to initate communication and don't respond to this email since this is an

                        automated service. Remember to send the location of your meetup if you do want to confirm the deal.
        
                        Best regards,
                        RideAlong Team";

                    try
                    {
#pragma warning disable CS8604
                        _mailKitService.SendEmail(username, "RideAlong Communication Confirmation", emailBody);
#pragma warning restore CS8604
                    }

                    catch
                    {
                        response.HasError = true;
                        response.ErrorMessage = " Emailing service failed";
                        _logService.CreateLogAsync("Info", "Business", "Communinication Establishment feature: " + response.ErrorMessage, accountUser.UserHash);
                    }
                    return response;
                }

            }
            else
            {
                response.HasError = true;
                response.ErrorMessage = " Emailing service is null";
                _logService.CreateLogAsync("Info", "Business", "Communinication Establishment feature: " + response.ErrorMessage, accountUser.UserHash);
            }
            return response;
        }
    }
}







