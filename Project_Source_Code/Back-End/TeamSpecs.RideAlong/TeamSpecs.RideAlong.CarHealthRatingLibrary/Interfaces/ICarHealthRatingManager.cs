using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CarHealthRatingLibrary.Interfaces
{
    public interface ICarHealthRatingManager
    {
        public IResponse RetrieveValidVehicleProfiles(IAccountUserModel user);

        public IResponse CallCalculateCarHealthRating(IAccountUserModel user, string vin);
    }
}
