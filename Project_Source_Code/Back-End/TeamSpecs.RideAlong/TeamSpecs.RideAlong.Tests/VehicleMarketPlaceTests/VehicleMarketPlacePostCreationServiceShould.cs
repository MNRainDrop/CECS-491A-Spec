using System.Diagnostics;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleMarketplace;

namespace TeamSpecs.RideAlong.TestingLibrary
{
    public class VehicleMarketPlacePostCreationServiceShould
    {

        [Fact]
        public void VehicleMarketPlacePostCreationServiceShould_CreateVehicleProfile_RequiredParametersPassedIn_ReturnValue_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            ConfigServiceJson configService = new ConfigServiceJson();
            var dao = new SqlServerDAO(configService);
            var _target = new SqlDbMarketplaceTarget(dao);

            IResponse response;

            //Parameters 
            string VIN = "VINTEST";
            int view = 1;
            string description = "This is test case 1";
            int status = 1;

            //Service 
            VehiceMarketplacePostCreationService Create = new VehiceMarketplacePostCreationService(_target);

            //Act 
            timer.Start();
            response = Create.CreateVehicleProfilePost(VIN, view, description, status);
            timer.Stop();


            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 5);
            Assert.True(response.HasError == false);



        }

    }
}
