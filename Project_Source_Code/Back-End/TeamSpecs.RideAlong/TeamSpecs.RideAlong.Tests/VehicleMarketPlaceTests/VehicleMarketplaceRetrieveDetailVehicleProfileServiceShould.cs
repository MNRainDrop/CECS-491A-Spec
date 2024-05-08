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
    public class VehicleMarketplaceRetrieveDetailVehicleProfileServiceShould
    {

        [Fact]
        public void VehicleMarketplaceRetrieveDetailVehicleProfileServiceShould_RetrieveAllPublicPost_RequiredParametersPassedIn_ReturnValue_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            ConfigServiceJson configService = new ConfigServiceJson();
            var dao = new SqlServerDAO(configService);
            var _target = new SqlDbMarketplaceTarget(dao);

            IResponse response;

            //Parameters 
            string VIN = "VIN5";
            /*int view = 1;
            string description = "This is test case 1";
            int status = 1;*/

            //Service 
            VehicleMarketplaceRetrieveDetailVehicleProfileService View = new VehicleMarketplaceRetrieveDetailVehicleProfileService(_target);

            //Act 
            timer.Start();
            response = View.RetrieveDetailVehicleProfile(VIN);
            timer.Stop();


            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 5);
            Assert.True(response.HasError == false);



        }
    }
}
