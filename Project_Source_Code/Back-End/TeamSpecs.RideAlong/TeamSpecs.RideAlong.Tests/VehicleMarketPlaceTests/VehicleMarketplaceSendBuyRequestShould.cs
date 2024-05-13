using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleMarketplace;

namespace TeamSpecs.RideAlong.TestingLibrary.VehicleMarketPlaceTests
{
    public class VehicleMarketplaceSendBuyRequestShould
    {

        [Fact]
        public void VehicleMarketplaceSendBuyRequestShould_RetrieveAllPublicPost_RequiredParametersPassedIn_ReturnValue_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            ConfigServiceJson configService = new ConfigServiceJson();
            var dao = new SqlServerDAO(configService);
            var _target = new SqlDbMarketplaceTarget(dao);
            var _mailKitService = new MailKitService(configService);

            IResponse response;

            //Parameters 
            string VIN = "VIN5";
            /*int view = 1;
            string description = "This is test case 1";
            int status = 1;*/

            //Service 
            VehiceMarketplaceSendBuyRequestService send = new VehiceMarketplaceSendBuyRequestService(_target, _mailKitService);

            //Act 
            timer.Start();
            response = send.SendBuyRequest(VIN,3);
            timer.Stop();


            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 5);
            Assert.True(response.HasError == false);



        }
    }
}
