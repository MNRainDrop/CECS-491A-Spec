namespace TeamSpecs.RideAlong.Model;

public interface IVendorVehicleModel : IVehicleProfileModel
{
    string Status { get; set; }
    DateTime PostingDate { get; set; }
    decimal Price { get; set; }
    DateTime PriceDate { get; set; }
    long Inquiries { get; set; }
    string Color { get; set; }
}
