using TeamSpecs.RideAlong.Model;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Services;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using TeamSpecs.RideAlong.VehicleMarketplace;
using Microsoft.Extensions.Configuration;
using static System.Collections.Specialized.BitVector32;
using TeamSpecs.RideAlong.Model.ConfigModels;

namespace TeamSpecs.RideAlong.TestingLibrary;

    public class VehicleMarketPlacePostDeletionServiceShould
    {
        private readonly ConnectionStrings _connStrings;
    public VehicleMarketPlacePostDeletionServiceShould()
    {
        var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); var configPath = Path.Combine(directory, "..","..","..", "..", "RideAlongConfiguration.json"); var configuration = new ConfigurationBuilder().AddJsonFile(configPath, optional: false, reloadOnChange: true).Build();
        var section = configuration.GetSection("ConnectionStrings");
        _connStrings = new ConnectionStrings(section["readOnly"], section["writeOnly"], section["admin"]);
    }
    [Fact]
        public void VehicleMarketPlacePostDeletionServiceShoul_DeletePostFromMarketplace_RequiredParametersPassedIn_ReturnValue_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            var _dao = new SqlServerDAO(_connStrings);
            var _target = new SqlDbMarketplaceTarget(_dao);

            IResponse response;

            //Parameters 
            string VIN = "1234567891012345";


            //Service 
            VehiceMarketplacePostDeletionService Delete = new VehiceMarketplacePostDeletionService(_target);

            //Act 
            timer.Start();
            response = Delete.DeletePostFromMarketplace(VIN);
            timer.Stop();


            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 5);
            Assert.True(response.HasError == false);


    }











    }

