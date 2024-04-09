namespace TeamSpecs.RideAlong.Model;

public class VendorVehicleModel : IVendorVehicleModel, IVehicleProfileModel
{
    public string VIN { get; set; }
    public long Owner_UID { get; set; }
    public string LicensePlate { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; } = 0;
    public string Status { get; set; }
    public DateTime PostingDate { get; set; }
    public decimal Price { get; set; }
    public DateTime PriceDate { get; set; }
    public long Inquiries { get; set; }
    public string Color { get; set; } = string.Empty;

    public VendorVehicleModel(string vin, long owner_UID, string licensePlate, string make = "", string model = "", int year = 0, string status = "Unassigned", DateTime postingDate = default(DateTime), decimal price = 0.0m, DateTime priceDate = default(DateTime), long inquiries = 0, string color = "")
    {
        VIN = vin;
        Owner_UID = owner_UID;
        LicensePlate = licensePlate;
        Make = make;
        Model = model;
        Year = year;
        Status = status;
        PostingDate = postingDate;
        Price = price;
        PriceDate = priceDate;
        Inquiries = inquiries;
        Color = color;
    }
}