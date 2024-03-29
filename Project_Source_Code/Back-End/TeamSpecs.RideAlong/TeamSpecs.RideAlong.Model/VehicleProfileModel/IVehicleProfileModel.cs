namespace TeamSpecs.RideAlong.Model;

public interface IVehicleProfileModel
{
    string VIN { get; set; }
    long Owner_UID { get; set; }
    string LicensePlate { get; set; }
    string? Make {  get; set; }
    string? Model {  get; set; }
    int? Year { get; set; }

    // Added 'Name' attribute since it is a viewable option when put on the marketplace
    string? Name { get; set; }
}
