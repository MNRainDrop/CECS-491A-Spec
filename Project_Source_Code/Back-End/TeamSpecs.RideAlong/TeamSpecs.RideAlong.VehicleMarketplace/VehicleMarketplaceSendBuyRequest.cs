using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.VehicleMarketplace;

namespace TeamSpecs.RideAlong.VehicleMarketplace
{
    public class VehicleMarketplaceSendBuyRequest : IVehiceMarketplaceSendBuyRequestService
    {
        private IMarketplaceTarget _target;

        public VehicleMarketplaceSendBuyRequest(IMarketplaceTarget target)
        {
            _target = target;
        }

        public IResponse SendBuyRequest(long uid, string vin, int price)
        {
            /*
            IResponse response;
            //Poppulate message to pass to target 
            string message = uid + " want to buy " + vin + " for " + price;
            Notification request = new Notification(uid, vin, price, message);
            response = _target.VehicleMarketplaceSendRequestService(request);

            return response;*/
            throw new NotImplementedException();
        }
    }
}
