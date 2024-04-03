using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Model.ConfigModels;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;
using static System.Collections.Specialized.BitVector32;

namespace TeamSpecs.RideAlong.TestingLibrary.VehicleProfileTests;

public class VehicleProfileDetailsRetrievalShould
{
    private readonly ConnectionStrings _connStrings;
    public VehicleProfileDetailsRetrievalShould()
    {
        var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); var configPath = Path.Combine(directory, "..","..","..", "..", "RideAlongConfiguration.json"); var configuration = new ConfigurationBuilder().AddJsonFile(configPath, optional: false, reloadOnChange: true).Build();
        var section = configuration.GetSection("ConnectionStrings");
        _connStrings = new ConnectionStrings(section["readOnly"], section["writeOnly"], section["admin"]);
    }
    [Fact]
    public void VehicleProfileDetailsRetrieval_ReadVehicleProfileDetailsFromDatabase_ValidVINPassedIn_OneVehicleProfileDetailsRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        IResponse response;

        var dao = new SqlServerDAO(_connStrings);
        var vehicleTarget = new SqlDbVehicleTarget(dao);

        var hashService = new HashService();
        var logTarget = new SqlDbLogTarget(dao);
        var logService = new LogService(logTarget, hashService);

        var retrievalService = new VehicleProfileDetailsRetrievalService(vehicleTarget, logService);

        // Create Test Objects
        var user = new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        var vehicleDetails = new VehicleDetailsModel(vehicle.VIN, "testColor", "testDescription");

        // Create Initial SQL
        try
        {
            var accountSql = $"INSERT INTO UserAccount (UserName, Userhash, Salt) VALUES ('{user.UserName}', '{user.UserHash}', {user.Salt})";
            var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
            var vehicleDetailsSql = $"INSERT INTO VehicleDetails (VIN, Color, Description) VALUES ('{vehicleDetails.VIN}', '{vehicleDetails.Color}', '{vehicleDetails.Description}')";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null),
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleDetailsSql, null)
            });
            var getUserID = $"SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'";
            var uid = dao.ExecuteReadOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(getUserID, null)
            });
            foreach (var item in uid)
            {
                user.UserId = (long)item[0];
                vehicle.Owner_UID = user.UserId;
            }
        }
        catch
        {
            // In case creating the initial sql data does not work
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Act
        try
        {
            timer.Start();
            response = retrievalService.retrieveVehicleDetails(vehicle, user);
            timer.Stop();
        }
        finally
        {
            var undoInsert = $"DELETE FROM UserAccount WHERE UID = '{user.UserId}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.NotNull(response);
        Assert.True(!response.HasError);
        Assert.NotNull(response.ReturnValue);
        Assert.True(response.ReturnValue.Count == 1);
        Assert.True(response.ReturnValue.FirstOrDefault() is not null);
        Assert.True(response.ReturnValue.FirstOrDefault() is VehicleDetailsModel);
        var returnedVehicle = response.ReturnValue.FirstOrDefault() as VehicleDetailsModel;
        Assert.NotNull(returnedVehicle);
        Assert.True(returnedVehicle.VIN == vehicle.VIN);
        Assert.True(returnedVehicle.Color == vehicleDetails.Color);
        Assert.True(returnedVehicle.Description == vehicleDetails.Description);
        #endregion
    }

    [Fact]
    public void VehicleProfileDetailsRetrieval_ReadVehicleProfileDetailsFromDatabase_ValidVINPassedIn_NoVehicleProfileDetailsRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        IResponse response;

        var dao = new SqlServerDAO(_connStrings);
        var vehicleTarget = new SqlDbVehicleTarget(dao);

        var hashService = new HashService();
        var logTarget = new SqlDbLogTarget(dao);
        var logService = new LogService(logTarget, hashService);

        var retrievalService = new VehicleProfileDetailsRetrievalService(vehicleTarget, logService);

        // Create Test Objects
        var user = new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        var vehicleDetails = new VehicleDetailsModel("NOTtestVin", "testColor", "testDescription");

        // Create Initial SQL
        try
        {
            var accountSql = $"INSERT INTO UserAccount (UserName, Userhash, Salt) VALUES ('{user.UserName}', '{user.UserHash}', {user.Salt})";
            var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
            var vehicleDetailsSql = $"INSERT INTO VehicleDetails (VIN, Color, Description) VALUES ('{vehicleDetails.VIN}', '{vehicleDetails.Color}', '{vehicleDetails.Description}')";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null),
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleDetailsSql, null)
            });
            var getUserID = $"SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'";
            var uid = dao.ExecuteReadOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(getUserID, null)
            });
            foreach (var item in uid)
            {
                user.UserId = (long)item[0];
                vehicle.Owner_UID = user.UserId;
            }
        }
        catch
        {
            // In case creating the initial sql data does not work
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}';" + 
                                $"DELETE FROM VehicleDetails WHERE VIN = '{vehicleDetails.VIN}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Act
        try
        {
            timer.Start();
            response = retrievalService.retrieveVehicleDetails(vehicle, user);
            timer.Stop();
        }
        finally
        {
            var undoInsert = $"DELETE FROM UserAccount WHERE UID = '{user.UserId}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.NotNull(response);
        Assert.True(!response.HasError);
        Assert.NotNull(response.ReturnValue);
        Assert.True(response.ReturnValue.Count == 0);
        #endregion
    }
}
