using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.CarNewsCenter;
using TeamSpecs.RideAlong.VehicleProfile;

namespace TeamSpecs.RideAlong.TestingLibrary
{
    public class CarNewsCenterViewVehicleNewsArticleServiceShould
    {
        [Fact]
        public async void CarNewsCenterViewVehicleNewsArticleServiceShould_GetNewsForAllVehicles_ValidUserAccountPassedIn_OneVehicleProfileRetrieved_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            var dao = new SqlServerDAO();
            var vehicleTarget = new SqlCarNewsCenterTarget(dao);

            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(dao);
            var logService = new LogService(logTarget, hashService);

            var retrievalService = new CarNewsCenterViewVehicleNewsArticleService(vehicleTarget, logService);

            //var numOfResults = 10;
            //var page = 1;

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
                response = await retrievalService.GetNewsForAllVehicles(user);
                timer.Stop();
            }
            finally
            {
                var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = ' {user.UserHash}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
            }
            #endregion

            #region Assert
            //Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            Assert.NotNull(response.ReturnValue);
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
        public async void CarNewsCenterViewVehicleNewsArticleServiceShould_GetNewsForAllVehicles_ValidUserAccountPassedIn_ThreeVehicleProfilesRetrieved_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            var dao = new SqlServerDAO();
            var vehicleTarget = new SqlCarNewsCenterTarget(dao);

            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(dao);
            var logService = new LogService(logTarget, hashService);

            var retrievalService = new CarNewsCenterViewVehicleNewsArticleService(vehicleTarget, logService);

            //var numOfResults = 10;
            //var page = 1;

            // Create Test Objects
            var user = new AccountUserModel("testUser")
            {
                Salt = 0,
                UserHash = "testUserHash",
            };
            var vehicle1 = new VehicleProfileModel("testVin1", 1, "test", "Toyota", "Camry", 0000);
            var vehicle2 = new VehicleProfileModel("testVin2", 1, "test", "Honda", "Civic", 0000);
            var vehicle3 = new VehicleProfileModel("testVin3", 1, "test", "Hyundai", "Sonata", 0000);

            // Create Initial SQL
            try
            {
                var accountSql = $"INSERT INTO UserAccount (UserName, Userhash, Salt) VALUES ('{user.UserName}', '{user.UserHash}', {user.Salt})";
                var vehicle1Sql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle1.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle1.LicensePlate}', '{vehicle1.Make}', '{vehicle1.Model}', {vehicle1.Year})";
                var vehicle2Sql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle2.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle2.LicensePlate}', '{vehicle2.Make}', '{vehicle2.Model}', {vehicle2.Year})";
                var vehicle3Sql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle3.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle3.LicensePlate}', '{vehicle3.Make}', '{vehicle3.Model}', {vehicle3.Year})";
                var writes = dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicle1Sql, null),
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicle2Sql, null),
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicle3Sql, null)
            });
                var getUserID = $"SELECT UID FROM UserAccount WHERE UserHash = '{user.UserHash}'";
                var uid = dao.ExecuteReadOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(getUserID, null)
            });
                foreach (var item in uid)
                {
                    user.UserId = (long)item[0];
                    vehicle1.Owner_UID = user.UserId;
                    vehicle2.Owner_UID = user.UserId;
                    vehicle3.Owner_UID = user.UserId;
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
                response = await retrievalService.GetNewsForAllVehicles(user);
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
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            Assert.NotNull(response.ReturnValue);
            #endregion
        }
    }
}
