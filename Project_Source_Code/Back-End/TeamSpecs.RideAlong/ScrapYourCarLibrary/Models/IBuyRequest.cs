namespace ScrapYourCarLibrary.Models
{
    public interface IBuyRequest
    {
        public long buyerUID { get; set; }
        public ICarPart part { get; set; }
        public string buyerMessage { get; set; }
        public string? status { get; set; }
        public string? contactInfo { get; set; }
    }
}
