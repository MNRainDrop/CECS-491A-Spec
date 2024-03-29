using Microsoft.IdentityModel.Tokens;
using TeamSpecs.RideAlong.RentalFleetLibrary.Interfaces;
using TeamSpecs.RideAlong.RentalFleetLibrary.Models;
using System.Reflection;
using System;
using System.Resources;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.RentalFleetLibrary
{
    public class RentalFleetService : IRentalFleetService
    {
        IRentalFleetTarget _target;
            
        public RentalFleetService(IRentalFleetTarget target)
        {
            _target = target;
        }

        public IResponse GetFleetFullModel(long uid)
        {
            IResponse response = _target.fetchFleetModels(uid);
            if (response.HasError)
            {
                if (response.ErrorMessage is null)
                {
                    response.ErrorMessage = $"Unknown error occurred at target layer or below for user {uid}";
                }
                else
                {
                    response.ErrorMessage += $"for user {uid}";
                }
            }
            if (response.ReturnValue is not null)
            {
                List<FleetInfoModel> primedModels = new List<FleetInfoModel>();
                List<object> models = response.ReturnValue.ToList();
                foreach(FleetFullModel model in models)
                {
                    if (model.status is null)
                    {
                        primedModels.Add(new FleetInfoModel(model.vin, 1, null, null));
                    }
                }
                IResponse response2 = _target.saveRentalFleetStatus(primedModels);
            }
            return response;
        }

        public IResponse setRentalStatus(FleetFullModel fleetInfo)
        {
            List<FleetInfoModel> fleetInfoModels = new List<FleetInfoModel>();
            IResponse response = _target.saveRentalFleetStatus(fleetInfoModels);
            if (response.HasError)
            {
                if (response.ErrorMessage.IsNullOrEmpty() || response.ErrorMessage is null)
                {
                    response.ErrorMessage = "Unknown error happened at target layer or below";
                }
            }
            return response;
        }
    }
}
