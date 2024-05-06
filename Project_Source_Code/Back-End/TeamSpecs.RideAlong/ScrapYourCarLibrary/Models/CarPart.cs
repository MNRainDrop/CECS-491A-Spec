namespace ScrapYourCarLibrary.Models
{
    public class CarPart : ICarPart
    {
        public CarPart(long ownerUID, int? year, long? partUID = null, string? partName = null, string? partNumber = null, string? make = null, string? model = null, string? associatedVin = null)
        {
            this.ownerUID = ownerUID;
            this.partUID = partUID;
            this.partName = partName;
            this.partNumber = partNumber;
            this.make = make;
            this.model = model;
            this.year = year;
            this.associatedVin = associatedVin;
        }
        public CarPart()
        {
            ownerUID = 0;
            partUID = null;
            partName = null;
            partNumber = null;
            make = null;
            model = null;
            year = null;
            associatedVin = null;
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
