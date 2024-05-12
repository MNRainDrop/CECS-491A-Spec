namespace TeamSpecs.RideAlong.Model.ServiceLogModel
{
    public class ServiceLogModel : IServiceLogModel
    {
        public int? ServiceLogID { get; set; }
        public string Category{get; set;}
        public string Part { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int? Mileage { get; set; }
        public string? VIN { get; set; }

        public ServiceLogModel(string category, string part, DateTime date, string description, int? mileage, string vin)
        {
            ServiceLogID = null;
            Category = category;
            Part = part;
            Date = date;
            Description = description;
            Mileage = mileage;
            VIN = vin;
        }

        /// <summary>
        /// For Car Health Rating to allow for collecting data about Service Log without unneccesary components
        /// </summary>
        /// <param name="part"></param>
        /// <param name="date"></param>
        /// <param name="mileage"></param>
        /// <param name="vin"></param>
        public ServiceLogModel(string part, DateTime date, int? mileage, string vin)
        {
            ServiceLogID = null;
            Category = "Transport Model";
            Part = part;
            Date = date;
            Description = "Transport Model";
            Mileage = mileage;
            VIN = vin;
        }

        public ServiceLogModel()
        {
            ServiceLogID = null;
            Category = string.Empty;
            Part = string.Empty;
            Date = DateTime.MinValue;
            Description = string.Empty;
            Mileage = 0;
            VIN = null;
        }
    }
}
