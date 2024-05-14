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

            var collection = response.ReturnValue;

            // Assuming the collection contains only one element
            if (collection.Count == 1)
            {
                var list = (List<object>)collection.First();

                // Check if the list has elements
                if (list.Count > 0)
                {
                    var username = (string)list[0];
                    // Now you can work with the first element as needed




                    if (response.ReturnValue != null && response.ReturnValue.Count > 0)
                    {

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
                    }
                }


                else
                {
                    // Handle the case where ReturnValue is null or empty
                    response.ErrorMessage = "email is null" + response.ErrorMessage;
                }




            }
            return response;
        }
    }
}
