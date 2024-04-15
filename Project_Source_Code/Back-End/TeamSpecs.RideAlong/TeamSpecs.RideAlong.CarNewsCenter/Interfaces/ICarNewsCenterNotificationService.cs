using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.CarNewsCenter.Interfaces
{
    public interface ICarNewsCenterNotificationService
    {
        IResponse NotificationAlert(INotification notification);
    }
}
