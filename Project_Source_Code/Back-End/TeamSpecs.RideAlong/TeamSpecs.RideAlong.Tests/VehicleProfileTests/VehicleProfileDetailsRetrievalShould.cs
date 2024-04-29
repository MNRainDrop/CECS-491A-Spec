using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.TestingLibrary.VehicleProfileTests;

public class VehicleProfileDetailsRetrievalShould
{
    private static readonly IGenericDAO dao = new SqlServerDAO();
    private static readonly ICRUDVehicleTarget vehicleTarget = new SqlDbVehicleTarget(dao);

    private static readonly IHashService hashService = new HashService();
    private static readonly ILogTarget logTarget = new SqlDbLogTarget(dao);
    private static readonly ILogService logService = new LogService(logTarget, hashService);

    private static readonly IVehicleProfileDetailsRetrievalService retrievalService = new VehicleProfileDetailsRetrievalService(vehicleTarget, logService);
    
    [Fact]
    public void VehicleProfileDetailsRetrieval_ReadVehicleProfileDetailsFromDatabase_ValidVINPassedIn_OneVehicleProfileDetailsRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        IResponse response;

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
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM VehicleProfile WHERE VIN LIKE '%{vehicle.VIN}%'";
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
            response = retrievalService.RetrieveVehicleDetails(vehicle, user);
            timer.Stop();
        }
        finally
        {
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM VehicleProfile WHERE VIN LIKE '%{vehicle.VIN}%'";
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
                                $"DELETE FROM VehicleDetails WHERE VIN = '{vehicleDetails.VIN}';" +
                                $"DELETE FROM VehicleProfile WHERE VIN LIKE '%{vehicle.VIN}%'";
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
            response = retrievalService.RetrieveVehicleDetails(vehicle, user);
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

    [Fact]
    public void VehicleProfileDetailsRetrieval_ReadVehicleProfileDetailsFromDatabase_InvalidVINPassedIn_OneVehicleProfileDetailsRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        // Create Test Objects
        var user = new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        var vehicleDetails = new VehicleDetailsModel("testVin", "testColor", "testDescription");

        var searchVehicle = new VehicleProfileModel("", 1, "test", "testMake", "testModel", 0000);

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
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM VehicleProfile WHERE VIN LIKE '%{vehicle.VIN}%'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Act
        try
        {
            Assert.ThrowsAny<Exception>(
                () => retrievalService.RetrieveVehicleDetails(searchVehicle, user)
            );
            
        }
        catch
        {
            Assert.Fail("Should throw exception");
        }
        finally
        {
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM VehicleProfile WHERE VIN LIKE '%{vehicle.VIN}%'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion
    }

    [Fact]
    public void VehicleProfileDetailsRetrieval_ReadVehicleProfileDetailsFromDatabase_InvalidUserHashPassedIn_OneVehicleProfileDetailsRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        // Create Test Objects
        var user = new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        var vehicleDetails = new VehicleDetailsModel("testVin", "testColor", "testDescription");

        var paramUser = user;
        user.UserHash = "";

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
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM VehicleProfile WHERE VIN LIKE '%{vehicle.VIN}%'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Act
        try
        {
            Assert.ThrowsAny<Exception>(
                () => retrievalService.RetrieveVehicleDetails(vehicle, user)
            );

        }
        catch
        {
            Assert.Fail("Should throw exception");
        }
        finally
        {
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM VehicleProfile WHERE VIN LIKE '%{vehicle.VIN}%'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion
    }
}
