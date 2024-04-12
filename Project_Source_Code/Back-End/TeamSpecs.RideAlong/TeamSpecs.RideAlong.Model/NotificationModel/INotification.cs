
using System.Numerics;

namespace TeamSpecs.RideAlong.Model
{
    public interface IRequest
    { 
        long UID { get; set; }
        string VIN { get; set; }
        int type {  get; set; }
        string description { get; set; }
    }
}
