namespace TeamSpecs.RideAlong.Model;

public class VehicleDetailsModel : IVehicleDetailsModel
{
    public string VIN { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public VehicleDetailsModel(string vin, string color = "", string description = "")
    {
        VIN = vin;
        Color = color;
        Description = description;
    }
}
