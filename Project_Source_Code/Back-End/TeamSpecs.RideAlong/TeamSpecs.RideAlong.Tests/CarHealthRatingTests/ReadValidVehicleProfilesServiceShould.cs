using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.CarHealthRatingLibrary;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.TestingLibrary.CarHealthRatingTests
{
    public class ReadValidVehicleProfilesServiceShould
    {
        [Fact]
        public void ReadValidVehicleProfiles_RetrieveVehicleProfilesinDB_ValidUserWithNoVehicleProfilesPassed_NoVehicleProfilesFound_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            var dao = new SqlServerDAO();
            var chrTarget = new SqlDBCarHealthRatingTarget(dao);

            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(dao);
            var logService = new LogService(logTarget, hashService);

            var retrievalService = new CarHealthRatingService(chrTarget, logService);

            // Create Test Objects
            var user = new AccountUserModel("testCarHealthRatingUser1")
            {
                Salt = 0,
                UserHash = "testCarHeathRatingUserHash1",
            };
            var vehicle = new VehicleProfileModel("testVin1", 1, "test", "testMake", "testModel", 0000);
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
                var undoInsert1 = $"DELETE FROM UserAccount WHERE UserName = '{user.UserName}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert1, null)
            });
            }
            #endregion

            #region Act   
                timer.Start();
                response = retrievalService.ValidVehicleProfileRetrievalService(user);
                timer.Stop();
            
                Thread.Sleep(1000); // Waiting for Log Task to finish

                var undoLogInsert = $"DELETE FROM Log WHERE UserHash = '{user.UserHash}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoLogInsert, null)
            });
                var undoInsert = $"DELETE FROM UserAccount WHERE UserName = '{user.UserName}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
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
        public void ReadValidVehicleProfiles_RetrieveVehicleProfilesInDB_ValidUserWithOneVehicleProfilesPassed_1VehicleProfileFound_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            var dao = new SqlServerDAO();
            var chrTarget = new SqlDBCarHealthRatingTarget(dao);

            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(dao);
            var logService = new LogService(logTarget, hashService);

            var retrievalService = new CarHealthRatingService(chrTarget, logService);

            long realUID;

            // Create Test Objects
            var user = new AccountUserModel("testCarHealthRatingUser2")
            {
                Salt = 0,
                UserHash = "testCarHeathRatingUserHash2",
            };
            var vehicle = new VehicleProfileModel("testVin2", 1, "test", "testMake", "testModel", 2017);

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

                var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null)
            });
            }
            catch
            {
                // In case creating the initial sql data does not work
                var undoInsert1 = $"DELETE FROM UserAccount WHERE UserName = '{user.UserName}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert1, null)
            });
            }
            #endregion

            #region Act   
            timer.Start();
            response = retrievalService.ValidVehicleProfileRetrievalService(user);
            timer.Stop();

            Thread.Sleep(1000); // Waiting for Log Task to finish

            var undoLogInsert = $"DELETE FROM Log WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoLogInsert, null)
            });

            var undoVehicleInsert = $"DELETE FROM VehicleProfile WHERE VIN = '{ vehicle.VIN}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoVehicleInsert, null)
            });

            var undoInsert = $"DELETE FROM UserAccount WHERE UserName = '{user.UserName}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
            #endregion

            #region Assert
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            Assert.NotNull(response.ReturnValue);
            Assert.True(response.ReturnValue.Count == 1);
            #endregion
        }

        [Fact]
        public void ReadValidVehicleProfiles_RetrieveVehicleProfilesInDB_ValidUserWithTenVehicleProfilesPassed_TenVehicleProfilesFound_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            var dao = new SqlServerDAO();
            var chrTarget = new SqlDBCarHealthRatingTarget(dao);

            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(dao);
            var logService = new LogService(logTarget, hashService);

            var retrievalService = new CarHealthRatingService(chrTarget, logService);

            long realUID;

            // Create Test Objects
            var user = new AccountUserModel("testCarHealthRatingUser3") 
            {
                Salt = 1, 
                UserHash = "newTestUserHash3", 
            };

            // Create 10 vehicle profiles
            var vehicles = new List<VehicleProfileModel>();
            for (int i = 1; i <= 10; i++)
            {
                var vehicle = new VehicleProfileModel($"testVin{i + 10}", i, $"test{i + 10}", "testMake", "testModel", 2017);
                vehicles.Add(vehicle);
            }

            // Insert UserAccount
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

            // Insert VehicleProfiles
            foreach (var vehicle in vehicles)
            {
                var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null)
        });
            }
            #endregion

            #region Act   
            timer.Start();
            response = retrievalService.ValidVehicleProfileRetrievalService(user);
            timer.Stop();

            Thread.Sleep(1000); // Waiting for Log Task to finish

            // Clean up database
            var undoLogInsert = $"DELETE FROM Log WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
    {
        KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoLogInsert, null)
    });

            // Clean up VehicleProfile entries
            foreach (var vehicle in vehicles)
            {
                var undoVehicleInsert = $"DELETE FROM VehicleProfile WHERE VIN = '{vehicle.VIN}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoVehicleInsert, null)
        });
            }

            // Clean up UserAccount entry
            var undoInsert = $"DELETE FROM UserAccount WHERE UserName = '{user.UserName}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
    {
        KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
    });
            #endregion

            #region Assert
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            Assert.NotNull(response.ReturnValue);
            Assert.True(response.ReturnValue.Count == 10); // Check for 10 vehicle profiles
            #endregion
        }


    }
}
