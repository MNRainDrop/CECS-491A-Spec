namespace TeamSpecs.RideAlong.Model;

public class VehicleDetailsModel : IVehicleDetailsModel
{
    public string VIN { get; set; }
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public string? Color { get; set; }
    public string? Description { get; set; }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public VehicleDetailsModel(string vin, string? color = null, string? description = null)
    {
        VIN = vin;
        Color = color;
        Description = description;
    }
}
