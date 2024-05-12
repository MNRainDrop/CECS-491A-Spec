using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using TeamSpecs.RideAlong.Model;
using Azure;
using TeamSpecs.RideAlong.Services;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using System.Drawing.Printing;

namespace TeamSpecs.RideAlong.UserAdministration
{
    public class AccountRetrievalService : IAccountRetrievalService
    {
        private SqlDbUserRetrievalTarget _target;
        private readonly IMailKitService _mailKitService;


        public AccountRetrievalService(SqlDbUserRetrievalTarget target, IMailKitService mailKitService)
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
                    var emailBody = $@"
                    Subject: Account Information Request

                    Dear ,

                    Here is your information ! Some info: {phone}, {addr}
            
                    P.S: we also log a bunch of stuff btw :) 
        
                    Best regards,
                    RideAlong Team";
                    response = _mailKitService.SendEmail(email, "RideAlong Registration Confirmation", emailBody);
                }
             
            }








            //Poppulate message to pass to target 
            

            //sending email
            //var email = temp2;
          


            return response;


        }

    }
}
