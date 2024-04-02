
using System.Numerics;

namespace TeamSpecs.RideAlong.Model
{
    public interface IRequest
    { 
        long UID { get; set; }
        string VIN { get; set; }
        int price {  get; set; }

        string message { get; set; }
    }
}
