namespace ScrapYourCarLibrary.Models
{
    public class Listing : IListing
    {
        public Listing(ICarPart part, float price, string? description = null)
        {
            this.part = part;
            this.price = price;
            this.description = description;
        }
        ICarPart part { get; set; }
        float price { get; set; }
        string? description { get; set; }
    }
}
