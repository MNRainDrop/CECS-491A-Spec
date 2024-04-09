using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.VehicleMarketplace
{
    internal class VehicleMarketplacePostRetrievalService : IVehiceMarketplacePostRetrievalService
    {
        private SqlDbMarketplaceTarget _target;

        public VehicleMarketplacePostRetrievalService(SqlDbMarketplaceTarget target)
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
