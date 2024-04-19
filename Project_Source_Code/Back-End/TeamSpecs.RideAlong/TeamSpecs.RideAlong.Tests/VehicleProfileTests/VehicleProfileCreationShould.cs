﻿using Azure;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.TestingLibrary.VehicleProfileTests;

public class VehicleProfileCreationShould
{
    private static readonly IGenericDAO dao = new SqlServerDAO();
    private static readonly ICreateVehicleTarget createVehicleTarget = new SqlDbVehicleTarget(dao);
    private static readonly IRetrieveVehiclesTarget retrieveVehicleTarget = new SqlDbVehicleTarget(dao);
    private static readonly IRetrieveVehicleDetailsTarget retrieveDetailsTarget = new SqlDbVehicleTarget(dao);

    private static readonly IHashService hashService = new HashService();
    private static readonly ILogTarget logTarget = new SqlDbLogTarget(dao);
    private static readonly ILogService logService = new LogService(logTarget, hashService);

    private static readonly IVehicleProfileCreationService creationService = new VehicleProfileCreationService(logService, createVehicleTarget);

    [Fact]
    public void VehicleProfileCreation_CreateVehicleProfileInDatabase_ValidParametersPassedIn_OneVehicleProfileAndVehicleDetailsWritten_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        IResponse response;
        IResponse vehicleInDB;
        IResponse detailsInDB;
        #endregion

        // Create Test Objects
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        var vehicledetails = new VehicleDetailsModel(vehicle.VIN);
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
            vehicleInDB = retrieveVehicleTarget.readVehicleProfileSql(new List<object>()
            {
                new KeyValuePair<string, long>("Owner_UID", user.UserId)
            }, 10, 1);
            detailsInDB = retrieveDetailsTarget.readVehicleProfileDetailsSql(new List<object>()
            {
                new KeyValuePair<string, string>("VIN", vehicle.VIN)
            });
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
        Assert.True(!vehicleInDB.HasError && !detailsInDB.HasError);
        Assert.True(vehicleInDB.ReturnValue is not null && detailsInDB.ReturnValue is not null);
        Assert.True(JsonConvert.SerializeObject(vehicleInDB.ReturnValue.First()) == JsonConvert.SerializeObject(vehicle));
        Assert.True(JsonConvert.SerializeObject(detailsInDB.ReturnValue.First()) == JsonConvert.SerializeObject(vehicledetails));
        #endregion
    }

    [Fact]
    public void VehicleProfileCreation_CreateVehicleProfileInDatabase_InvalidVinPassedIn_NoVehicleProfileAndVehicleDetailsWritten_Fail()
    {
        #region Arrange
        IResponse vehicleInDB;
        IResponse detailsInDB;
        #endregion

        // Create Test Objects
        var vehicle = new VehicleProfileModel("", 1, "test", "testMake", "testModel", 0000);
        var vehicledetails = new VehicleDetailsModel(vehicle.VIN);
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

        #region Act and Assert
        try
        {
            Assert.ThrowsAny<Exception>(
                () => creationService.createVehicleProfile(vehicle.VIN, vehicle.LicensePlate, vehicle.Make, vehicle.Model, vehicle.Year, vehicledetails.Color, vehicledetails.Description, user)
            );
        }
        catch
        {
            Assert.Fail("Should throw error");
        }
        finally
        {
            vehicleInDB = retrieveVehicleTarget.readVehicleProfileSql(new List<object>()
            {
                new KeyValuePair<string, long>("Owner_UID", user.UserId)
            }, 10, 1);
            detailsInDB = retrieveDetailsTarget.readVehicleProfileDetailsSql(new List<object>()
            {
                new KeyValuePair<string, string>("VIN", vehicle.VIN)
            });
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Assert
        Assert.True(!vehicleInDB.HasError && !detailsInDB.HasError);
        Assert.True(vehicleInDB.ReturnValue is not null && detailsInDB.ReturnValue is not null);
        Assert.True(vehicleInDB.ReturnValue.Count == 0 && detailsInDB.ReturnValue.Count == 0);
        #endregion
    }

    [Fact]
    public void VehicleProfileCreation_CreateVehicleProfileInDatabase_InvalidLicensePlatePassedIn_NoVehicleProfileAndVehicleDetailsWritten_Fail()
    {
        #region Arrange
        IResponse vehicleInDB;
        IResponse detailsInDB;
        #endregion

        // Create Test Objects
        var vehicle = new VehicleProfileModel("testVin", 1, "", "testMake", "testModel", 0000);
        var vehicledetails = new VehicleDetailsModel(vehicle.VIN);
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

        #region Act and Assert
        try
        {
            Assert.ThrowsAny<Exception>(
                () => creationService.createVehicleProfile(vehicle.VIN, vehicle.LicensePlate, vehicle.Make, vehicle.Model, vehicle.Year, vehicledetails.Color, vehicledetails.Description, user)
            );
        }
        catch
        {
            Assert.Fail("Should throw error");
        }
        finally
        {
            vehicleInDB = retrieveVehicleTarget.readVehicleProfileSql(new List<object>()
            {
                new KeyValuePair<string, long>("Owner_UID", user.UserId)
            }, 10, 1);
            detailsInDB = retrieveDetailsTarget.readVehicleProfileDetailsSql(new List<object>()
            {
                new KeyValuePair<string, string>("VIN", vehicle.VIN)
            });
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Assert
        Assert.True(!vehicleInDB.HasError && !detailsInDB.HasError);
        Assert.True(vehicleInDB.ReturnValue is not null && detailsInDB.ReturnValue is not null);
        Assert.True(vehicleInDB.ReturnValue.Count == 0 && detailsInDB.ReturnValue.Count == 0);
        #endregion
    }

    [Fact]
    public void VehicleProfileCreation_CreateVehicleProfileInDatabase_InvalidUserAccountPassedIn_NoVehicleProfileAndVehicleDetailsWritten_Fail()
    {
        #region Arrange
        IResponse vehicleInDB;
        IResponse detailsInDB;
        #endregion

        // Create Test Objects
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        var vehicledetails = new VehicleDetailsModel(vehicle.VIN);
        var user = new AccountUserModel("")
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

        #region Act and Assert
        try
        {
            Assert.ThrowsAny<Exception>(
                () => creationService.createVehicleProfile(vehicle.VIN, vehicle.LicensePlate, vehicle.Make, vehicle.Model, vehicle.Year, vehicledetails.Color, vehicledetails.Description, user)
            );
        }
        catch
        {
            Assert.Fail("Should throw error");
        }
        finally
        {
            vehicleInDB = retrieveVehicleTarget.readVehicleProfileSql(new List<object>()
            {
                new KeyValuePair<string, long>("Owner_UID", user.UserId)
            }, 10, 1);
            detailsInDB = retrieveDetailsTarget.readVehicleProfileDetailsSql(new List<object>()
            {
                new KeyValuePair<string, string>("VIN", vehicle.VIN)
            });
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Assert
        Assert.True(!vehicleInDB.HasError && !detailsInDB.HasError);
        Assert.True(vehicleInDB.ReturnValue is not null && detailsInDB.ReturnValue is not null);
        Assert.True(vehicleInDB.ReturnValue.Count == 0 && detailsInDB.ReturnValue.Count == 0);
        #endregion
    }
}
