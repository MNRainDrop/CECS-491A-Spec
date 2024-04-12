using static System.Runtime.InteropServices.JavaScript.JSType;
using TeamSpecs.RideAlong.Model;
using System.Transactions;

public interface ICarNewsCenterTarget
{
    IResponse GetsAllVehicles(ICollection<object> searchParameter);
    IResponse UpdateNotification(INotification notification);
}