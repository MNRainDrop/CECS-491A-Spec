namespace TeamSpecs.RideAlong.Model;

public class VehicleMarketplaceDetailModel : IVehicleMarketplaceDetailModel
{
    public string VIN { get; set; }
    public long? Owner_UID { get; set; }
    public string LicensePlate { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; } = 0000;
    public DateTime DateCreated { get; set; }
    public int ViewStatus { get; set; }
    public string Description { get; set; }
    public int MarketplaceStatus { get; set; }



    public VehicleMarketplaceDetailModel(string vin, long? owner_UID, string licensePlate, string make = "", string model = "", int year = 0, DateTime date = new DateTime(), int view = 0, string description = "", int status = 0)
    {
        VIN = vin;
        Owner_UID = owner_UID;
        LicensePlate = licensePlate;
        Make = make;
        Model = model;
        Year = year;
        DateCreated = date; 
        ViewStatus = view;
        Description = description;      
        MarketplaceStatus = status;
    }
}
