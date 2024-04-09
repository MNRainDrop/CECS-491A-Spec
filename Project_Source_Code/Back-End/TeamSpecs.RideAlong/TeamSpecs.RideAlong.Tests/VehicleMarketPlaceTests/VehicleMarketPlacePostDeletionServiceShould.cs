using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleMarketplace;

namespace TeamSpecs.RideAlong.TestingLibrary;

public class VehicleMarketPlacePostDeletionServiceShould
{
    [Fact]
    public void VehicleMarketPlacePostDeletionServiceShoul_DeletePostFromMarketplace_RequiredParametersPassedIn_ReturnValue_Pass()
    {
        //Arrange 
        var timer = new Stopwatch();
        var _dao = new SqlServerDAO();
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

