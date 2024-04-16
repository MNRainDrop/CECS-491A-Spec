namespace TeamSpecs.RideAlong.Model
{
    public class Notification : INotification
    {
        public long UID { get; set; }
        public string VIN { get; set; }
        public string type { get; set; }
        public string description { get; set; }

        public Notification(long uid, string Vin, string Type, string Description) 
        {
            UID = uid;
            VIN = Vin;
            type = Type;
            description = Description;
        }
    }
  
}
