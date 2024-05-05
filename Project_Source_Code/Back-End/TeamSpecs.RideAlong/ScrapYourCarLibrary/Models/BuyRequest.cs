namespace ScrapYourCarLibrary.Models
{
    public class BuyRequest : IBuyRequest
    {
        public BuyRequest(long buyerUID, ICarPart part, string buyerMessage = "No Message Provided", string? status = null, string? contactInfo = null)
        {
            this.buyerUID = buyerUID;
            this.part = part;
            this.buyerMessage = buyerMessage;
            this.status = status;
            this.contactInfo = contactInfo;
        }
        public long buyerUID { get; set; }
        public ICarPart part { get; set; }
        public string buyerMessage { get; set; }
        public string? status { get; set; }
        public string? contactInfo { get; set; }
    }
}
