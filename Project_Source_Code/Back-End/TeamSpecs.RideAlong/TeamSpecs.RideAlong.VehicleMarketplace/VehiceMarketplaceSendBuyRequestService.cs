using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.VehicleMarketplace;
using static System.Net.WebRequestMethods;

namespace TeamSpecs.RideAlong.VehicleMarketplace
{
    public class VehiceMarketplaceSendBuyRequestService : IVehiceMarketplaceSendBuyRequestService
    {
        private IMarketplaceTarget _target;
        private readonly IMailKitService _mailKitService;

        public VehiceMarketplaceSendBuyRequestService(IMarketplaceTarget target, IMailKitService mailKitService)
        {
            _target = target;
            _mailKitService = mailKitService;
        }

        public IResponse SendBuyRequest(string vin, int price)
        {

            IResponse response = new Response()
            {
                HasError = true
            };
            
            //Poppulate message to pass to target 
            var emailBody = $@"
            Subject: Buy Request

            Dear ,

            I want to buy your car !
        
            Best regards,
            RideAlong Team";


            var email = _target.VehicleMarketplaceSendRequestService(vin);
            if (email.ReturnValue is not null)
            {
                var _email= email.ReturnValue.First() as object[];
                if (_email is not null)
                {
                    var _temp = _email[0] as string;
                    if (!string.IsNullOrWhiteSpace(_temp))
                    {
                        response = _mailKitService.SendEmail(_temp, "RideAlong Registration Confirmation", emailBody);
                    }
                    
                }

            }


            return response;
        }
    }
}
