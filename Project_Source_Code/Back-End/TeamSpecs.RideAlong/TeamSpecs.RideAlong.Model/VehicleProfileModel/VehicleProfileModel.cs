namespace TeamSpecs.RideAlong.Model;

public class VehicleProfileModel : IVehicleProfileModel
{
    public string VIN { get; set; }
    public long Owner_UID { get; set; }
    public string LicensePlate { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; } = 0000;

    public VehicleProfileModel(string vin, long owner_UID, string licensePlate, string make = "", string model = "", int year = 0000)
    {
        VIN = vin;
        Owner_UID = owner_UID;
        LicensePlate = licensePlate;
        Make = make;
        Model = model;
        Year = year;
    }
}
