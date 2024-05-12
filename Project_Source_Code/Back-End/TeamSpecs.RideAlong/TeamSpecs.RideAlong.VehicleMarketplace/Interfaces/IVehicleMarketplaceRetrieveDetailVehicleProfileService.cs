using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleMarketplace;

public interface IVehicleMarketplaceRetrieveDetailVehicleProfileService
{
    IResponse RetrieveDetailVehicleProfile(string VIN);
}

