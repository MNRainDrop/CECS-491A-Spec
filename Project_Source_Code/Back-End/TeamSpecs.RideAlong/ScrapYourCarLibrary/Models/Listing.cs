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
        public ICarPart part { get; set; }
        public float price { get; set; }
        public string? description { get; set; }
    }
}
