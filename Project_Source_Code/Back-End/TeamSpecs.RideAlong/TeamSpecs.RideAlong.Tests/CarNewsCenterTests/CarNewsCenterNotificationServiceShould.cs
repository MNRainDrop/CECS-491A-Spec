using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.CarNewsCenter;
using TeamSpecs.RideAlong.LoggingLibrary;

namespace TeamSpecs.RideAlong.TestingLibrary.CarNewsCenterTests
{
    public class CarNewsCenterNotificationServiceShould
    {
        [Fact]
        public void CarNewsCenterNotificationServiceShould_NotificationAlert_RequiredParametersPassedIn_ReturnValue_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            var _dao = new SqlServerDAO();
            var _target = new SqlCarNewsCenterTarget(_dao);
            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(_dao);
            var logService = new LogService(logTarget, hashService);

            IResponse response;

            //Parameters 
            long UID = 0;
            string VIN = "324324324";
            int view = 1;
            string description = "For sale because it suks";
            string type = "Renting";
            INotification notification = new Notification(UID,VIN,type,description);

            //Service 
            CarNewsCenterNotificationService Notif = new CarNewsCenterNotificationService(_target,logService);

            //Act 
            timer.Start();
            response = Notif.NotificationAlert(notification);
            timer.Stop();


            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 5);
            Assert.True(response.HasError == false);



        }

    }
}
