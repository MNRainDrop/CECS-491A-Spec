namespace TeamSpecs.RideAlong.Model;

internal interface IVendorVehicleModel
{
    int Status { get; set; }
    DateTime PostingDate { get; set; }
    double Price { get; set; }
    DateTime PriceDate { get; set; }
    long Inquiries { get; set; }
}
