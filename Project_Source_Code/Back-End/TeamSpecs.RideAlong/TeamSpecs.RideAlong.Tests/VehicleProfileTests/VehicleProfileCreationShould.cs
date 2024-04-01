using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.TestingLibrary.VehicleProfileTests;

public class VehicleProfileCreationShould
{
    [Fact]
    public void VehicleProfileRetrieval_ReadVehicleProfilesFromDatabase_ValidUserAccountPassedIn_OneVehicleProfileRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        IResponse response;

        var dao = new SqlServerDAO();
        var vehicleTarget = new SqlDbVehicleTarget(dao);

        var hashService = new HashService();
        var logTarget = new SqlDbLogTarget(dao);
        var logService = new LogService(logTarget, hashService);
        
        var creationService = new VehicleProfileCreationService(logService, vehicleTarget);
        #endregion

        // Create Test Objects
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        var vehicledetails = new VehicleDetailsModel("testVin");
        var user = new AccountUserModel("testUsername")
        {
            UserHash = "123",
            Salt = 1,
            UserId = 1
        };

        var accountSql = $"INSERT INTO UserAccount (UserName, Userhash, Salt) VALUES ('{user.UserName}', '{user.UserHash}', {user.Salt})";
        dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null)
        });
        var getUserID = $"SELECT UID FROM UserAccount WHERE UserHash = '{user.UserHash}'";
        var uid = dao.ExecuteReadOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(getUserID, null)
        });
        foreach (var item in uid)
        {
            user.UserId = (long)item[0];
            vehicle.Owner_UID = user.UserId;
        }

        #region Act
        try
        {
            timer.Start();
            response = creationService.createVehicleProfile(vehicle.VIN, vehicle.LicensePlate, vehicle.Make, vehicle.Model, vehicle.Year, vehicledetails.Color, vehicledetails.Description, user);
            timer.Stop();
        }
        finally
        {
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Assert
        Assert.True(response is not null);
        Assert.True(!response.HasError);
        Assert.True(response.ReturnValue is not null);
        Assert.True((int)response.ReturnValue.First() == 2);
        #endregion
    }
}
