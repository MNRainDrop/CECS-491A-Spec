using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.VehicleMarketplace.Managers
{
    public class VehicleMarketplaceManager : IVehicleMarketplaceManager
    {
        private IVehiceMarketplacePostCreationService _postCreationService;
        private IVehiceMarketplacePostDeletionService _postDeletionService;


        //Constructor 
        public VehicleMarketplaceManager(IVehiceMarketplacePostCreationService postCreationService, IVehiceMarketplacePostDeletionService postDeletionService)
        {
            _postCreationService = postCreationService;
            _postDeletionService = postDeletionService;
        }

        //Checking business rules and calling Post Creation service 
        public IResponse CreateVehicleProfilePost(string VIN, int view, string Description, int MarketplaceStatus)
        {
            IResponse response;
            response = _postCreationService.CreateVehicleProfilePost(VIN, view, Description, MarketplaceStatus);

            return response;

        }

        //Checking business rules and calling Post Deletion service 
        public IResponse DeletePostFromMarketplace(string VIN)
        {
            IResponse response;
            response = _postDeletionService.DeletePostFromMarketplace(VIN);
            return response;

        }

        //Checking business rules and calling Post Retrieveal service 
        public IResponse RetrieveAllPublicPost()
        {
            /* VehicleMarketplacePostRetrievalService retrievalService = new VehicleMarketplacePostRetrievalService(); 
             retrievalService.RetrieveAllPublicPost();   */
            throw new NotImplementedException();

        }

        //Checking business rules and calling SendBuyRequest service 
        public IResponse SendBuyRequest(long uid, string vin, int price)
        {
            /*VehicleMarketplaceSendBuyRequest sendBuyRequestService = new VehicleMarketplaceSendBuyRequest();
            sendBuyRequestService.SendBuyRequest();*/
            throw new NotImplementedException();
        }
    }
}
