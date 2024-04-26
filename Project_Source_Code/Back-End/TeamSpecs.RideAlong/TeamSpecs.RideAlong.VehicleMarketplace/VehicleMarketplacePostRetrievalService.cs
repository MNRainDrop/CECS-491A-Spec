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

        public IResponse RetrieveAllPublicPost()
        {
            IResponse response;
            response = _target.ReadAllPublicVehicleProfileSql();

            return response;


        }
    }
}
