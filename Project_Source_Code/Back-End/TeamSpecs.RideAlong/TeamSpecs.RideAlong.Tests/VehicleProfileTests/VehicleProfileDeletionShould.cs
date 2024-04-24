﻿using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.TestingLibrary.VehicleProfileTests;

public class VehicleProfileDeletionShould
{
    [Fact]
    public void VehicleProfileDeletion_DeleteVehicleProfileInDatabase_ValidParametersPassedIn_OneVehicleProfileAndOneVehicleDetailsDeleted_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        IResponse response;

        var dao = new SqlServerDAO();
        var vehicleTarget = new SqlDbVehicleTarget(dao);

        var hashService = new HashService();
        var logTarget = new SqlDbLogTarget(dao);
        var logService = new LogService(logTarget, hashService);

        var deletionService = new VehicleProfileDeletionService(vehicleTarget, logService);

        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        var vehicledetails = new VehicleDetailsModel("testVin");
        var user = new AccountUserModel("testUsername")
        {
            UserHash = "123",
            Salt = 1,
            UserId = 1
        };
        var numOfResults = 1;
        var changedRows = 1;

        List<object[]> databaseItem;
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
        #endregion

        #region Act
        try
        {
            timer.Start();
            response = deletionService.deleteVehicleProfile(vehicle, user);
            timer.Stop();
        }
        finally
        {
            var databaseItemSql = $"SELECT vp.VIN, Owner_UID, LicensePlate, Make, Model, Year, Color, Description FROM VehicleProfile as vp INNER JOIN VehicleDetails as vd on vp.VIN = vd.VIN WHERE vp.VIN = '{vehicle.VIN}'";
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
        Assert.True(databaseItem.Count == 0);
        #endregion
    }

    [Fact]
    public void VehicleProfileDeletion_DeleteVehicleProfileInDatabase_InvalidParametersPassedIn_NoVehicleProfileAndOneVehicleDetailsDeleted_Pass()
    {
        #region Arrange
        #endregion

        #region Act
        #endregion

        #region Assert
        #endregion
    }
}