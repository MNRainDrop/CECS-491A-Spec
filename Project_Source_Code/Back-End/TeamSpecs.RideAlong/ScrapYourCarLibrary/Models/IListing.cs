namespace ScrapYourCarLibrary.Models
{
    public interface IListing
    {
        ICarPart part { get; set; }
        float price { get; set; }
        string? description { get; set; }
    }
}
