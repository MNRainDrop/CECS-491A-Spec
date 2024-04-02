﻿using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.VehicleMarketplace;

public interface IVehicleMarketplaceManager
{
    IResponse CreateVehicleProfilePost(string VIN, int view, string Description, int MarketplaceStatus);

    IResponse DeletePostFromMarketplace(string VIN);

    IResponse RetrieveAllPublicPost();
    IResponse SendBuyRequest(long uid, string vin, int price);


}
