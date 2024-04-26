using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleMarketplace;

namespace TeamSpecs.RideAlong.TestingLibrary
{
    public class VehicleMarketPlacePostRetrievalServiceShould
    {

        [Fact]
        public void VehicleMarketPlacePostRetrievalServiceShould_RetrieveAllPublicPost_RequiredParametersPassedIn_ReturnValue_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            var _dao = new SqlServerDAO();
            var _target = new SqlDbMarketplaceTarget(_dao);

            IResponse response;

            //Parameters 
            string VIN = "VIN2";
            int view = 1;
            string description = "This is test case 1";
            int status = 1;

            //Service 
            VehicleMarketplacePostRetrievalService View = new VehicleMarketplacePostRetrievalService(_target);

            //Act 
            timer.Start();
            response = View.RetrieveAllPublicPost();
            timer.Stop();


            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 5);
            Assert.True(response.HasError == false);



        }

    }
}
