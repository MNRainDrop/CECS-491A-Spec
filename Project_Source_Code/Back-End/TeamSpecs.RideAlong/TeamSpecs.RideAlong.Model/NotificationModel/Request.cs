namespace TeamSpecs.RideAlong.Model
{
    public class Request : IRequest
    {
        public long UID { get; set; }
        public string VIN { get; set; }
        public int price { get; set; }
        public string message { get; set; }

        public Request(long uid, string Vin, int Price, string Message) 
        {
            UID = uid;
            VIN = Vin;
            price = Price;
            message = Message; 
        }
    }
  
}
