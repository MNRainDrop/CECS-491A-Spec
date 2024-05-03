using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.TestingLibrary.VehicleProfileTests;

public class VehicleProfileRetrievalShould
{
    private static readonly IConfigServiceJson configService = new ConfigServiceJson();
    private static readonly IGenericDAO dao = new SqlServerDAO(configService);
    private static readonly ICRUDVehicleTarget vehicleTarget = new SqlDbVehicleTarget(dao);

    private static readonly IHashService hashService = new HashService();
    private static readonly ILogTarget logTarget = new SqlDbLogTarget(dao);
    private static readonly ILogService logService = new LogService(logTarget, hashService);

    private static readonly IVehicleProfileRetrievalService retrievalService = new VehicleProfileRetrievalService(vehicleTarget, logService);

    [Fact]
    public void VehicleProfileRetrieval_ReadVehicleProfilesFromDatabase_ValidUserAccountPassedIn_OneVehicleProfileRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        IResponse response;

        var numOfResults = 10;
        var page = 1;

        // Create Test Objects
        var user = new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);

        // Create Initial SQL
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

            var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null)
            });
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
            response = retrievalService.RetrieveVehicleProfilesForUser(user, numOfResults, page);
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
        Assert.True(response.ReturnValue.Count <= numOfResults);
        Assert.True(response.ReturnValue.Count == 1);
        Assert.True(response.ReturnValue.FirstOrDefault() is not null);
        Assert.True(response.ReturnValue.FirstOrDefault() is VehicleProfileModel);
        var returnedVehicle = response.ReturnValue.FirstOrDefault() as VehicleProfileModel;
        Assert.NotNull(returnedVehicle);
        Assert.True(returnedVehicle.VIN == vehicle.VIN);
        Assert.True(returnedVehicle.Make == vehicle.Make);
        Assert.True(returnedVehicle.Model == vehicle.Model);
        Assert.True(returnedVehicle.Year == vehicle.Year);
        Assert.True(returnedVehicle.LicensePlate == vehicle.LicensePlate);
        Assert.True(returnedVehicle.Owner_UID == vehicle.Owner_UID);
        #endregion
    }

    [Fact]
    public void VehicleProfileRetrieval_ReadVehicleProfilesFromDatabase_ValidUserAccountPassedIn_NoVehicleProfileRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        IResponse response;

        var numOfResults = 10;
        var page = 1;

        // Create Test Objects
        var user = new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);
        long realUID;

        // Create Initial SQL
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
                realUID = (long)item[0];
                user.UserId = realUID;
            }
            vehicle.Owner_UID = user.UserId;
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
            response = retrievalService.RetrieveVehicleProfilesForUser(user, numOfResults, page);
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
        Assert.True(response.ReturnValue.Count <= numOfResults);
        Assert.True(response.ReturnValue.Count == 0);
        #endregion
    }

    [Fact]
    public void VehicleProfileRetrieval_ReadVehicleProfilesFromDatabase_ValidUserAccountPassedIn_ThreeVehicleProfileRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        var responseList = new List<IResponse>();

        var numOfResults = 3;

        // Create Test Objects
        var user = new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };

        var baseVehicle = new VehicleProfileModel($"testVin", user.UserId, "test", "testMake", "testModel", 0000);

        var vehicleList = new List<VehicleProfileModel>();

        // Create Initial SQL
        try
        {
            var accountSql = $"INSERT INTO UserAccount (UserName, Userhash, Salt) VALUES ('{user.UserName}', '{user.UserHash}', {user.Salt})";
            var writes = dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
            });
            var getUserID = $"SELECT UID FROM UserAccount WHERE UserHash = '{user.UserHash}'";
            var uid = dao.ExecuteReadOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(getUserID, null)
            });

            foreach (var item in uid)
            {
                user.UserId = (long)item[0];
            }

            for (int i = 0; i < numOfResults; i++)
            {
                var vehicle = new VehicleProfileModel($"{baseVehicle.VIN}{i}", user.UserId, baseVehicle.LicensePlate, baseVehicle.Make, baseVehicle.Model, baseVehicle.Year);

                var sql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', '{vehicle.Owner_UID}', '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
                var vehicleSql = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
                {
                    KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
                };

                vehicleList.Add(vehicle);
                var vehiclewrites = dao.ExecuteWriteOnly(vehicleSql);
                Thread.Sleep(5);
            }
        }
        catch
        {
            // In case creating the initial sql data does not work
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM VehicleProfile WHERE VIN LIKE '%{baseVehicle.VIN}%'";
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
            responseList.Add(retrievalService.RetrieveVehicleProfilesForUser(user, numOfResults, 1));
            timer.Stop();
        }
        finally
        {
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM VehicleProfile WHERE VIN LIKE '%{baseVehicle.VIN}%'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Assert
        var totalResults = new List<VehicleProfileModel>();
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        foreach (var response in responseList)
        {
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            Assert.NotNull(response.ReturnValue);
            Assert.True(response.ReturnValue.Count <= numOfResults);
            Assert.True(response.ReturnValue.FirstOrDefault() is not null);
            Assert.True(response.ReturnValue.FirstOrDefault() is VehicleProfileModel);
            foreach (VehicleProfileModel vehicle in response.ReturnValue)
            {
                totalResults.Add(vehicle);
            }
        }
        for (var i = 0; i < totalResults.Count; i++)
        {
            Assert.True(totalResults[i].VIN == vehicleList[i].VIN);
            Assert.True(totalResults[i].Make == vehicleList[i].Make);
            Assert.True(totalResults[i].Model == vehicleList[i].Model);
            Assert.True(totalResults[i].Year == vehicleList[i].Year);
            Assert.True(totalResults[i].LicensePlate == vehicleList[i].LicensePlate);
            Assert.True(totalResults[i].Owner_UID == vehicleList[i].Owner_UID);
        }
        #endregion
    }

    [Fact]
    public void VehicleProfileRetrieval_ReadVehicleProfilesFromDatabase_ValidUserAccountPassedIn_VehicleProfilesPaginated_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        var responseList = new List<IResponse>();

        var numOfResults = 10;

        // Create Test Objects
        var user = new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };

        var baseVehicle = new VehicleProfileModel("testVin", user.UserId, "test", "testMake", "testMode", 0000);

        var vehicleList = new List<VehicleProfileModel>();

        // Create Initial SQL
        try
        {
            var accountSql = $"INSERT INTO UserAccount (UserName, Userhash, Salt) VALUES ('{user.UserName}', '{user.UserHash}', {user.Salt})";
            var writes = dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
            });
            var getUserID = $"SELECT UID FROM UserAccount WHERE UserHash = '{user.UserHash}'";
            var uid = dao.ExecuteReadOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(getUserID, null)
            });

            foreach (var item in uid)
            {
                user.UserId = (long)item[0];
            }


            for (int i = 0; i < 100; i++)
            {
                var vehicle = new VehicleProfileModel($"{baseVehicle.VIN}{i}", user.UserId, baseVehicle.LicensePlate, baseVehicle.Make, baseVehicle.Model, baseVehicle.Year);

                var sql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', '{vehicle.Owner_UID}', '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
                var vehicleSql = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
                {
                    KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
                };

                vehicleList.Add(vehicle);
                var vehiclewrites = dao.ExecuteWriteOnly(vehicleSql);
                Thread.Sleep(5);
            }
        }
        catch
        {
            // In case creating the initial sql data does not work
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM VehicleProfile WHERE VIN LIKE '%{baseVehicle.VIN}%'";
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
            for(var i = 1; i <= 10; i++)
            {
                responseList.Add(retrievalService.RetrieveVehicleProfilesForUser(user, numOfResults, i));
            }
            
            timer.Stop();
        }
        finally
        {
            var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM VehicleProfile WHERE VIN LIKE '%{baseVehicle.VIN}%'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
        }
        #endregion

        #region Assert
        var totalResults = new List<VehicleProfileModel>();
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        foreach (var response in responseList)
        {
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            Assert.NotNull(response.ReturnValue);
            Assert.True(response.ReturnValue.Count <= numOfResults);
            Assert.True(response.ReturnValue.FirstOrDefault() is not null);
            Assert.True(response.ReturnValue.FirstOrDefault() is VehicleProfileModel);
            foreach (VehicleProfileModel vehicle in response.ReturnValue)
            {
                totalResults.Add(vehicle);
            }
        }
        for (var i = 0; i < totalResults.Count; i++)
        {
            Assert.True(totalResults[i].VIN == vehicleList[i].VIN);
            Assert.True(totalResults[i].Make == vehicleList[i].Make);
            Assert.True(totalResults[i].Model == vehicleList[i].Model);
            Assert.True(totalResults[i].Year == vehicleList[i].Year);
            Assert.True(totalResults[i].LicensePlate == vehicleList[i].LicensePlate);
            Assert.True(totalResults[i].Owner_UID == vehicleList[i].Owner_UID);
        }
        #endregion
    }

    [Fact]
    public void VehicleProfileRetrieval_ReadVehicleProfilesFromDatabase_InvalidUserNamePassedIn_OneVehicleProfileRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        var numOfResults = 10;
        var page = 1;

        // Create Test Objects
        var user = new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };
        var searchingUser = user;
        searchingUser.UserName = "";
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);

        // Create Initial SQL
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

            var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null)
            });
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

        #region Act and Assert
        try
        {
            Assert.ThrowsAny<Exception>( 
                () => retrievalService.RetrieveVehicleProfilesForUser(user, numOfResults, page)
            );
        }
        catch
        {
            Assert.Fail("Should throw an error");
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
    public void VehicleProfileRetrieval_ReadVehicleProfilesFromDatabase_InvalidUserHashPassedIn_OneVehicleProfileRetrieved_Pass()
    {
        #region Arrange
        var timer = new Stopwatch();

        var numOfResults = 10;
        var page = 1;

        // Create Test Objects
        var user = new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };
        var searchingUser = user;
        searchingUser.UserHash = "";
        var vehicle = new VehicleProfileModel("testVin", 1, "test", "testMake", "testModel", 0000);

        // Create Initial SQL
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

            var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null)
            });
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

        #region Act and Assert
        try
        {
            Assert.ThrowsAny<Exception>(
                () => retrievalService.RetrieveVehicleProfilesForUser(user, numOfResults, page)
            );
        }
        catch
        {
            Assert.Fail("Should throw an error");
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
