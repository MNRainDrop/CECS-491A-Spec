using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Model.ConfigModels;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleMarketplace;
using static System.Collections.Specialized.BitVector32;

namespace TeamSpecs.RideAlong.TestingLibrary
{
    public class VehicleMarketPlacePostCreationServiceShould
    {
        private readonly ConnectionStrings _connStrings;
        public VehicleMarketPlacePostCreationServiceShould()
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); var configPath = Path.Combine(directory, "..","..","..", "..", "RideAlongConfiguration.json"); var configuration = new ConfigurationBuilder().AddJsonFile(configPath, optional: false, reloadOnChange: true).Build();
            var section = configuration.GetSection("ConnectionStrings");
            _connStrings = new ConnectionStrings(section["readOnly"], section["writeOnly"], section["admin"]);
        }
        [Fact]
        public void VehicleMarketPlacePostCreationServiceShould_CreateVehicleProfile_RequiredParametersPassedIn_ReturnValue_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            var _dao = new SqlServerDAO(_connStrings);
            var _target = new SqlDbMarketplaceTarget(_dao);

            IResponse response;

            //Parameters 
            string VIN = "1234567891012345";
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
