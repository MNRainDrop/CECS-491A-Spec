using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.VehicleMarketplace
{
    public class VehicleMarketplaceRetrieveDetailVehicleProfileService : IVehicleMarketplaceRetrieveDetailVehicleProfileService
    {
        private IMarketplaceTarget _target;

        public VehicleMarketplaceRetrieveDetailVehicleProfileService(IMarketplaceTarget target)
        {
            _target = target;
        }

        public IResponse RetrieveDetailVehicleProfile(string VIN)
        {
            IResponse response;
            response = _target.RetrieveDetailVehicleProfileSql(VIN);

            return response;


        }
    }
}
