using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CarNewsCenter;

public interface ICarNewsCenterManager
{
    Task<IResponse> GetNewsForAllVehicles(IAccountUserModel userAccount);

    IResponse NotificationAlert(INotification notification);

}
