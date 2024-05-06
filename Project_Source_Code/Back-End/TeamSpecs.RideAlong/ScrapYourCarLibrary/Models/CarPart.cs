namespace ScrapYourCarLibrary.Models
{
    public class CarPart : ICarPart
    {
        public CarPart(long ownerUID, int? year, long? partUID = null, string? partName = null, string? partNumber = null, string? partMake = null, string? partModel = null, string? associatedVin = null)
        {
            this.ownerUID = ownerUID;
            this.partUID = partUID;
            this.partName = partName;
            this.partNumber = partNumber;
            this.make = partMake;
            this.model = partModel;
            this.year = year;
            this.associatedVin = associatedVin;
        }
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
