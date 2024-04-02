using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.DataAccess;

namespace TeamSpecs.RideAlong.VehicleMarketplace;

public class VehiceMarketplacePostCreationService: IVehiceMarketplacePostCreationService
{
    private SqlDbMarketplaceTarget _target;

    public VehiceMarketplacePostCreationService(SqlDbMarketplaceTarget target)
    {
        _target = target;
    }
    public IResponse CreateVehicleProfilePost(string VIN, int view, string Description, int MarketplaceStatus)
    {
        #region Validate Parameters
        if (VIN is null)
        {
            throw new ArgumentNullException(VIN);
        }
        if (Description is null)
        {
            throw new ArgumentNullException(Description);
        }
        #endregion

        IResponse response;
        //Passing parameters to target to generate SQL 
        response = _target.UploadVehicleToMarketplace(VIN, view, Description, MarketplaceStatus);

        return response;

    }

}

