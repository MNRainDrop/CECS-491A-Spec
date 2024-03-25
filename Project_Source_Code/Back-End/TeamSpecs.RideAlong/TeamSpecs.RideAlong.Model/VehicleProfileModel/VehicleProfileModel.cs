namespace TeamSpecs.RideAlong.Model;

public class VehicleProfileModel : IVehicleProfileModel
{
    public string VIN { get; set; }
    public long Owner_UID { get; set; }
    public string LicensePlate { get; set; }
    public string? Make { get; set; } = null;
    public string? Model { get; set; } = null;
    public int? Year { get; set; }

    // Added 'Name' attribute since it is a viewable option when put on the marketplace
    public string? Name { get; set; } = null;

    public VehicleProfileModel(string vin, long owner_UID, string licensePlate, string? make = null, string? model = null, int? year = null, string? name = null)
    {
        VIN = vin;
        Owner_UID = owner_UID;
        LicensePlate = licensePlate;
        Make = make;
        Model = model;
        Year = year;
        Name = name;
    }

}
