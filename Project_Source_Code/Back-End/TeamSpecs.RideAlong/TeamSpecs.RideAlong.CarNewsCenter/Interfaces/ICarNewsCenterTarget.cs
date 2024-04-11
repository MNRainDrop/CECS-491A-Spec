using static System.Runtime.InteropServices.JavaScript.JSType;
using TeamSpecs.RideAlong.Model;

public interface ICarNewsCenterTarget
{
    IResponse GetsAllVehicles(BigInt UID);

}