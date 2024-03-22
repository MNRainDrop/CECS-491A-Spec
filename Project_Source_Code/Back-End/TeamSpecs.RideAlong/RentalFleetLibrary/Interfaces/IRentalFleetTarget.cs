using TeamSpecs.RideAlong.RentalFleetLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.RentalFleetLibrary.Interfaces
{
    public interface IRentalFleetTarget
    {
        IResponse fetchFleetModels(long uid);
        IResponse saveRentalFleetStatus(List<FleetInfoModel> fleetInfoModels);
    }
}
