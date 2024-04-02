namespace TeamSpecs.RideAlong.Model;

public class VehicleDetailsModel : IVehicleDetailsModel
{
    public string VIN { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }

    public VehicleDetailsModel(string vin, string? color = null, string? description = null)
    {
        VIN = vin;
        Color = color;
        Description = description;
    }
}
