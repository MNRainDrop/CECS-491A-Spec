using System.Diagnostics;
using TeamSpecs.RideAlong.ConfigService;
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
        ConfigServiceJson configService = new ConfigServiceJson();
        var dao = new SqlServerDAO(configService);
        var _target = new SqlDbMarketplaceTarget(dao);

        IResponse response;

        //Parameters 
        string VIN = "VIN8";


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

