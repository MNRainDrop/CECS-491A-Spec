namespace ScrapYourCarLibrary.Models
{
    public interface ICarPart
    {
        public long? partUID { get; set; }
        public long ownerUID { get; set; }
        public string? partName { get; set; }
        public string? partNumber { get; set; }
        public string? make { get; set; }
        public string? model { get; set; }
        public int? year { get; set; }
        public string? associatedVin { get; set; }
    }
}
