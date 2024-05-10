using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace TeamSpecs.RideAlong.VehicleMarketplace.Managers
{
    public class VehicleMarketplaceManager : IVehicleMarketplaceManager
    {
        private readonly IVehiceMarketplacePostCreationService _postCreationService;
        private readonly IVehiceMarketplacePostDeletionService _postDeletionService;
        private readonly IVehiceMarketplacePostRetrievalService _vehiceMarketplacePostRetrievalService;
        private readonly IVehicleMarketplaceRetrieveDetailVehicleProfileService _vehicleMarketplaceRetrieveDetailVehicleProfileService;
        private readonly IVehiceMarketplaceSendBuyRequestService _vehiceMarketplaceSendBuyRequestService;
        private readonly IConfigServiceJson _config;
        private readonly int _numOfResults;


        //Constructor 
        public VehicleMarketplaceManager(IVehiceMarketplacePostCreationService postCreationService, IVehiceMarketplacePostDeletionService postDeletionService, IVehiceMarketplacePostRetrievalService vehiceMarketplacePostRetrievalService, IVehicleMarketplaceRetrieveDetailVehicleProfileService vehicleMarketplaceRetrieveDetail, IVehiceMarketplaceSendBuyRequestService vehiceMarketplaceSendBuyRequestService, IConfigServiceJson configServiceJson)
        {
            _postCreationService = postCreationService;
            _postDeletionService = postDeletionService;
            _vehiceMarketplacePostRetrievalService = vehiceMarketplacePostRetrievalService;
            _vehicleMarketplaceRetrieveDetailVehicleProfileService = vehicleMarketplaceRetrieveDetail;
            _vehiceMarketplaceSendBuyRequestService = vehiceMarketplaceSendBuyRequestService;
            _config = configServiceJson;
            _numOfResults = _config.GetConfig().VEHICLE_MARKETPLACE_MANAGER.NUMOFRESULTS;
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
        public IResponse RetrieveAllPublicPost(int page)
        {
            IResponse response;
            response = _vehiceMarketplacePostRetrievalService.RetrieveAllPublicPost(_numOfResults,page);
            return response;

        }

        public IResponse RetrieveDetailVehicleProfile(string VIN) 
        {
            IResponse response;
            response = _vehicleMarketplaceRetrieveDetailVehicleProfileService.RetrieveDetailVehicleProfile(VIN);
            return response;
        }

        //Checking business rules and calling SendBuyRequest service 
        public IResponse SendBuyRequest(long uid, string vin, int price)
        {
            IResponse response;
            response = _vehiceMarketplaceSendBuyRequestService.SendBuyRequest(vin,price);
            return response ;
        }
    }
}
