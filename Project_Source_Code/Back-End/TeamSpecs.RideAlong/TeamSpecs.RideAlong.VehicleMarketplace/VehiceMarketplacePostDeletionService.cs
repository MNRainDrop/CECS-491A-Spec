using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.DataAccess;

namespace TeamSpecs.RideAlong.VehicleMarketplace;
public class VehiceMarketplacePostDeletionService : IVehiceMarketplacePostDeletionService
{
    private SqlDbMarketplaceTarget _target;

    public VehiceMarketplacePostDeletionService(SqlDbMarketplaceTarget target)
    {
        _target = target;
    }
    public IResponse DeletePostFromMarketplace(string VIN)
    {
        #region Validate Parameters
        if (VIN is null)
        {
            throw new ArgumentNullException(VIN);
        }
        #endregion

        IResponse response;
        //Passing parameters to target to generate SQL 
        response = _target.DeleteVehicleFromMarketplace(VIN);

        return response;

    }

}

