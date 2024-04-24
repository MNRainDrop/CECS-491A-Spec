using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CarHealthRatingLibrary.Interfaces
{
    public interface ICarHealthRatingService
    {
        public IResponse ValidVehicleProfileRetrievalService(IAccountUserModel userAccount);

        public IResponse CalculateCarHealthRatingService(IAccountUserModel userAccount, string vin);
    }
}
