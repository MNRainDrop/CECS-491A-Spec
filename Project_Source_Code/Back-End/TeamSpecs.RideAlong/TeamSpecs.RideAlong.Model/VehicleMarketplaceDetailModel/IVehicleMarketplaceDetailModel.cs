namespace TeamSpecs.RideAlong.Model;

public interface IVehicleMarketplaceDetailModel
{
    string VIN { get; set; }
    public long? Owner_UID { get; set; }
    string LicensePlate { get; set; }
    string Make { get; set; }
    string Model { get; set; }
    int Year { get; set; }
    DateTime DateCreated { get; set; }
    int ViewStatus { get; set; }
    string Description { get; set; }
    int MarketplaceStatus { get; set; }
}
