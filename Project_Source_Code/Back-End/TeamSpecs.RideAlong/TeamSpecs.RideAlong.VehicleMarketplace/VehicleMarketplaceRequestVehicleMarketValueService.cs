using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.VehicleMarketplace
{
    public  class VehicleMarketplaceRequestVehicleMarketValueService
    {

        private SqlDbMarketplaceTarget _target;

        public VehicleMarketplaceRequestVehicleMarketValueService(SqlDbMarketplaceTarget target)
        {
            target = _target;
        }
    }
}
