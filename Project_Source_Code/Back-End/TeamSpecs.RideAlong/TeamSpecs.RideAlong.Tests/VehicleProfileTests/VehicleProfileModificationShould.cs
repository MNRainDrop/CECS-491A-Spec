using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.TestingLibrary.VehicleProfileTests;

public class VehicleProfileModificationShould
{
    private static readonly IGenericDAO dao = new SqlServerDAO();
    private static readonly ICRUDVehicleTarget vehicleTarget = new SqlDbVehicleTarget(dao);

    private static readonly IHashService hashService = new HashService();
    private static readonly ILogTarget logTarget = new SqlDbLogTarget(dao);
    private static readonly ILogService logService = new LogService(logTarget, hashService);

    private static readonly IVehicleProfileModificationService modificationService = new VehicleProfileModificationService(vehicleTarget, logService);

    [Fact]
    public void VehicleProfileModificationShould_ModifyVehicleProfileAndVehicleDetailsInDatabase_ValidParametersPassedIn_OneVehicleProfileAndVehicleDetailsUpdated_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        IResponse response;

        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        var vehicledetails = new VehicleDetailsModel("testVin");
        var user = new AccountUserModel("testUsername")
        {
            UserHash = "123",
            Salt = 1,
            UserId = 1
        };
        var numOfResults = 1;
        var changedRows = 2;
        try
        {
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
            var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', {vehicle.Owner_UID}, '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
            var vehicleDetailSql = $"INSERT INTO VehicleDetails (VIN) VALUES ('{vehicledetails.VIN}')";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null),
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleDetailSql, null)
            });
        }
        catch
        {
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }

        var updatedvehicle = new VehicleProfileModel("testVin", vehicle.Owner_UID, "NEWTEST", "NEWMAKE", "NEWMODEL", 727);
        var updatedvehicledetails = new VehicleDetailsModel("testVin", "NEWCOLOR", "NEWDESCRIPTION");
        List<object[]> databaseItem;
        #endregion

        #region Act
        try
        {
            timer.Start();
            response = modificationService.ModifyVehicleProfile(updatedvehicle, updatedvehicledetails, user);
            timer.Stop();
        }
        finally
        {
            var databaseItemSql = $"SELECT vp.VIN, Owner_UID, LicensePlate, Make, Model, Year, Color, Description FROM VehicleProfile as vp INNER JOIN VehicleDetails as vd on vp.VIN = vd.VIN WHERE vp.VIN = '{updatedvehicle.VIN}'";
            databaseItem = dao.ExecuteReadOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(databaseItemSql, null)
            });

            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(!response.HasError);
        Assert.NotNull(response.ReturnValue);
        Assert.True(response.ReturnValue.Count == numOfResults);
        Assert.True((int)response.ReturnValue.First() == changedRows);
        Assert.True(updatedvehicle.VIN == (string)databaseItem.First()[0]);
        Assert.True(updatedvehicle.Owner_UID == (long)databaseItem.First()[1]);
        Assert.True(updatedvehicle.LicensePlate == (string)databaseItem.First()[2]);
        Assert.True(updatedvehicle.Make == (string)databaseItem.First()[3]);
        Assert.True(updatedvehicle.Model == (string)databaseItem.First()[4]);
        Assert.True(updatedvehicle.Year == (int)databaseItem.First()[5]);
        Assert.True(updatedvehicledetails.Color == (string)databaseItem.First()[6]);
        Assert.True(updatedvehicledetails.Description == (string)databaseItem.First()[7]);
        #endregion
    }

    [Fact]
    public void VehicleProfileModificationShould_ModifyVehicleProfileAndVehicleDetailsInDatabase_NoExistingVehiclePassedIn_NoVehicleProfileAndVehicleDetailsUpdated_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        IResponse response = new Response();

        var user = new AccountUserModel("testUsername")
        {
            UserHash = "123",
            Salt = 1,
            UserId = 1
        };
        var numOfResults = 1;
        var changedRows = 2;
        
        var updatedvehicle = new VehicleProfileModel("VinNotInDB", user.UserId, "NEWTEST", "NEWMAKE", "NEWMODEL", 727);
        var updatedvehicledetails = new VehicleDetailsModel("VinNotInDB", "NEWCOLOR", "NEWDESCRIPTION");
        #endregion

        #region Act
        try
        {
            timer.Start();
            response = modificationService.ModifyVehicleProfile(updatedvehicle, updatedvehicledetails, user);
            timer.Stop();
        }
        catch
        {
            Assert.Fail("Should not throw exception.");
        }
        #endregion

        #region Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(!response.HasError);
        Assert.NotNull(response.ReturnValue);
        Assert.True(response.ReturnValue.Count == numOfResults);
        Assert.True((int)response.ReturnValue.First() == changedRows);
        #endregion
    }

    [Fact]
    public void VehicleProfileModificationShould_ModifyVehicleProfileAndVehicleDetailsInDatabase_InvalidVehiclePassedIn_NoVehicleProfileAndVehicleDetailsUpdated_Pass()
    {
        #region Arrange
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        var vehicledetails = new VehicleDetailsModel("testVin");
        var user = new AccountUserModel("testUsername")
        {
            UserHash = "123",
            Salt = 1,
            UserId = 1
        };

        try
        {
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
            var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', {vehicle.Owner_UID}, '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
            var vehicleDetailSql = $"INSERT INTO VehicleDetails (VIN) VALUES ('{vehicledetails.VIN}')";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null),
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleDetailSql, null)
            });
        }
        catch
        {
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }

        var updatedvehicle = new VehicleProfileModel("", user.UserId, "NEWTEST", "NEWMAKE", "NEWMODEL", 727);
        var updatedvehicledetails = new VehicleDetailsModel("VinNotInDB", "NEWCOLOR", "NEWDESCRIPTION");
        #endregion
        
        #region Act and Assert
        try
        {
            Assert.ThrowsAny<Exception>(
                () => modificationService.ModifyVehicleProfile(updatedvehicle, updatedvehicledetails, user)
            );
        }
        catch
        {
            Assert.Fail("Should throw exception.");
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
    }
}
