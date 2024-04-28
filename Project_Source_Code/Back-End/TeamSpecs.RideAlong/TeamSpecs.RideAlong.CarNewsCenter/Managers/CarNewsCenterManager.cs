using TeamSpecs.RideAlong.CarNewsCenter.Interfaces;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CarNewsCenter.Managers
{
    public class CarNewsCenterManager : ICarNewsCenterManager
    {
        private ICarNewsCenterViewVehicleNewsArticleServices _ViewVehicleNewsArticleServices;
        private ICarNewsCenterNotificationService _NotifiationService;

        public CarNewsCenterManager(ICarNewsCenterViewVehicleNewsArticleServices ViewVehicleNewsArticleServices, ICarNewsCenterNotificationService NotifiationService)
        {
            _ViewVehicleNewsArticleServices = ViewVehicleNewsArticleServices;
            _NotifiationService = NotifiationService;
        }

        public async Task<IResponse> GetNewsForAllVehicles(IAccountUserModel userAccount)
        {
            IResponse response;
            if (userAccount != null)
            {
                response = await _ViewVehicleNewsArticleServices.GetNewsForAllVehicles(userAccount);
            }
            else
            {
                response = new Response();
            }
            return response;
        }
        public IResponse NotificationAlert(INotification notif)
        {
            IResponse response;
            if (notif != null)
            {
                response = _NotifiationService.NotificationAlert(notif);
            }
            else {
                response = new Response(); 
            } 
                
            return response;
        }
    }
}
