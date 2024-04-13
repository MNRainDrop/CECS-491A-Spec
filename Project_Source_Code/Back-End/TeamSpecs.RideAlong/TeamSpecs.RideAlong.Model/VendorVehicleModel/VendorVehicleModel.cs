namespace TeamSpecs.RideAlong.Model;

public class VendorVehicleModel : IVendorVehicleModel
{
    public int Status { get; set; }
    public DateTime PostingDate { get; set; }
    public double Price { get; set; }
    public DateTime PriceDate { get; set; }
    public long Inquiries { get; set; }

    public VendorVehicleModel(int status, DateTime postingDate, double price, DateTime priceDate, long inquiries)
    {
        Status = status;
        PostingDate = postingDate;
        Price = price;
        PriceDate = priceDate;
        Inquiries = inquiries;
    }
}