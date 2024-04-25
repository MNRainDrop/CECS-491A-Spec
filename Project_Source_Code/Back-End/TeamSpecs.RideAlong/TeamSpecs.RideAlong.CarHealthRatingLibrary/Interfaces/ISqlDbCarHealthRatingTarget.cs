using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CarHealthRatingLibrary.Interfaces
{
    public interface ISqlDbCarHealthRatingTarget
    {
        public IResponse ReadValidVehicleProfiles(long userID);
        public IResponse ReadServiceLogs(string vin);
    }
}
