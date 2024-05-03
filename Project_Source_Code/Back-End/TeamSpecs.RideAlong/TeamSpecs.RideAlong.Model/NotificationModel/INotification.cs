
using System.Numerics;

namespace TeamSpecs.RideAlong.Model
{
    public interface INotification
    { 
        long UID { get; set; }
        string VIN { get; set; }
        string type {  get; set; }
        string description { get; set; }
    }
}
