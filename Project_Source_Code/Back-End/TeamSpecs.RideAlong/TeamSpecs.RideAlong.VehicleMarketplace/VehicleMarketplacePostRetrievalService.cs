using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.VehicleMarketplace
{
    public class VehicleMarketplacePostRetrievalService : IVehiceMarketplacePostRetrievalService
    {
        private IMarketplaceTarget _target;

        public VehicleMarketplacePostRetrievalService(IMarketplaceTarget target)
        {
            _target = target;
        }

        public IResponse RetrieveAllPublicPost(int numOfResults, int page)
        {
            IResponse response;
            response = _target.ReadAllPublicVehicleProfileSql(numOfResults,page);

            return response;


        }
    }
}
