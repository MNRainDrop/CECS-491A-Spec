using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using TeamSpecs.RideAlong.Model;
using Azure;
using TeamSpecs.RideAlong.Services;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TeamSpecs.RideAlong.DataAccess;
using MimeKit;
using System.Text.Json;
using TeamSpecs.RideAlong.ConfigService.ConfigModels;


namespace TeamSpecs.RideAlong.UserAdministration
{
    public class AccountRetrievalService : IAccountRetrievalService
    {
        private ISqlDbUserRetrievalTarget _target;
        private readonly IMailKitService _mailKitService;


        public AccountRetrievalService(ISqlDbUserRetrievalTarget target, IMailKitService mailKitService)
        {
            _target = target;
            _mailKitService = mailKitService;
        }

        public IResponse RetrieveAccount(long uid)
        {
            IResponse response;
            response = _target.RetrieveAllUserInformation(uid);


            if (response.ReturnValue is not null)
            {
                var temp = response.ReturnValue.FirstOrDefault() as UserDataRequestModel;
                if (temp != null)
                {
                    var email = temp.UserName;
                    var phone = temp.PhoneNumber;
                    var addr = temp.Address;
                    MimeMessage message = new MimeMessage();

                    message.Body = new TextPart("plain")
                    {
                        Text = "Notification for Account Retrieval !"
                    };

                    var emailBody = $@"
                    Subject: Account Information Request

                    Dear ,

                    Here is your information ! Some info: {phone}, {addr}
            
                    P.S: we also log a bunch of stuff btw :) 
        
                    Best regards,
                    RideAlong Team";

                    //Writing to file
                    var currentDir = AppDomain.CurrentDomain.BaseDirectory;
                    string filePath = Path.Combine(currentDir, @"..\..\..\..\Report.txt");
                    using (StreamWriter outputFile = new StreamWriter(filePath, false))
                    {
                        outputFile.WriteLine(emailBody);
                    }


                    response = _mailKitService.SendEmailwithAttachments(email, "RideAlong Registration Confirmation", emailBody, filePath);
                }

            }

            return response;
        }

        public IResponse RetrieveAllAccount()
        {
            IResponse response;
            response = _target.RetrieveAllAccountInformation();
            return response;
        }


    }
}
