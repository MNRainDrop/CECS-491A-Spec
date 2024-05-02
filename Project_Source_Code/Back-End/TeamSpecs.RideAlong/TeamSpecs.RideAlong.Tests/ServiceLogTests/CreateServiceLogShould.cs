using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Model.ServiceLogModel;
using TeamSpecs.RideAlong.ServiceLog;
using TeamSpecs.RideAlong.ServiceLog.Interfaces;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.TestingLibrary.ServiceLogTests
{

    public class CreateServiceLogShould
    {
        private static readonly IConfigServiceJson configService = new ConfigServiceJson();
        private static readonly IGenericDAO dao = new SqlServerDAO(configService);
        private static readonly IHashService hashService = new HashService();
        private static readonly ILogTarget logTarget = new SqlDbLogTarget(dao);
        private static readonly ILogService logService = new LogService(logTarget, hashService);

        private static readonly ISqlDbServiceLogTarget sqlTarget = new SqlDbServiceLogServiceTarget(dao);
        private static readonly IServiceLogService serviceLogService = new ServiceLogService(logService, sqlTarget);

        public static TheoryData<IServiceLogModel> ValidServiceLogs =
            new()
            {
                {new ServiceLogModel("Maintenance", "Oil", new DateTime(2016, 3, 1), "Recieved Oil change", 100000, "SlVin101") },
                {new ServiceLogModel("Repair", "Car windshield broken", new DateTime(2016, 5, 1), "Fixed at local auto dealership", 101000, "SlVin101") },
                {new ServiceLogModel("Modification", "Spoilers", new DateTime(2016, 6, 1), "Added red spoilers to car", null, "SlVin101") },
                {new ServiceLogModel("Maintenance", "Brakes", new DateTime(2017, 5, 1), "Recieved Oil change", 110000, "SlVin101") },
                {new ServiceLogModel("Maintenance", "Oil", new DateTime(2018, 3, 1), "Recieved Oil change", 120000, "SlVin101") },
                {new ServiceLogModel("Maintenance", "Oil", new DateTime(2019, 3, 1), "Recieved Oil change", 130000, "SlVin101") },
                {new ServiceLogModel("Maintenance", "Oil", new DateTime(2020, 3, 1), "Recieved Oil change", 140000, "SlVin101") },
                {new ServiceLogModel("Maintenance", "Oil", new DateTime(2021, 3, 1), "Recieved Oil change", 150000, "SlVin101") },
                {new ServiceLogModel("Maintenance", "Oil", new DateTime(2022, 3, 1), "Recieved Oil change", 160000, "SlVin101") },
                {new ServiceLogModel("Damage Report", "Car Door", new DateTime(2023, 4, 10), "T - boned by other car", 170000, "SlVin101")}
            };


        [Fact]
        public void ServiceLogCreation_OneUserValidServiceLogEntryParametersPassed_OneServiceLogEntryWrittenToSqlDB_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            long realUID;

            #region Creating Test objects
            var user = new AccountUserModel("testServiceLogUser1")
            {
                Salt = 0,
                UserHash = "ServiceLogUserHash1",
            };

            var vehicle = new VehicleProfileModel
                ("SlVin100", 1, "SL00001", "testMake", "testModel", 2010);

            var serviceLog = new ServiceLogModel
                ("Maintenance", "Coolant", new DateTime(2017, 4, 2), "Recieved Coolant", 900, "SlVin100");
            #endregion

            try
            {
                #region Inserting Values to Database
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

                var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
                {
                    KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null)
                });

                #endregion

            }
            catch
            {
                #region In case creating the initial sql data does not work
                var undoInsert1 = $"DELETE FROM UserAccount WHERE UserName = '{user.UserName}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert1, null)
            });
                #endregion
            }
            #endregion

            #region Act

            timer.Start();
            response = serviceLogService.CreateServiceLog(serviceLog, user);
            timer.Stop();

            Thread.Sleep(1000); // Waiting for Log Async Task to finish

            #region Clean up DB
            var undoLogInsert = $"DELETE FROM Log WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoLogInsert, null)
            });

            var undoServiceLogInsert = $"Delete FROM ServiceLog WHERE VIN = '{serviceLog.VIN}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoServiceLogInsert, null)
            });

            var undoVehicleInsert = $"DELETE FROM VehicleProfile WHERE VIN = '{vehicle.VIN}'";
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

            #endregion

            #region Assert
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ValidServiceLogs))]
        public void SerivceLogCreation_MultipleServiceLogInserted_OneUserValid10ServiceLogEntriesPassed_OneServiceLogsWrittenToSqlDB_Pass(IServiceLogModel serviceLog)
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            long realUID;

            #region Creating Test objects
            var user = new AccountUserModel("testServiceLogUser2")
            {
                Salt = 0,
                UserHash = "ServiceLogUserHash1",
            };

            var vehicle = new VehicleProfileModel
                ("SlVin101", 1, "SL00002", "testMake", "testModel", 2015);
            #endregion

            try
            {
                #region Inserting Values to Database
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

                var vehicleSql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', (SELECT UID FROM UserAccount WHERE UserName = '{user.UserName}'), '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
                {
                    KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleSql, null)
                });

                #endregion

            }
            catch
            {
                #region In case creating the initial sql data does not work
                var undoInsert1 = $"DELETE FROM UserAccount WHERE UserName = '{user.UserName}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert1, null)
            });
                #endregion
            }

            #endregion

            #region Act

            timer.Start();
            response = serviceLogService.CreateServiceLog(serviceLog, user);
            timer.Stop();

            Thread.Sleep(1000); // Waiting for Log Async Task to finish

            #region Clean up DB
            var undoLogInsert = $"DELETE FROM Log WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoLogInsert, null)
            });

            var undoServiceLogInsert = $"Delete FROM ServiceLog WHERE VIN = '{serviceLog.VIN}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoServiceLogInsert, null)
            });

            var undoVehicleInsert = $"DELETE FROM VehicleProfile WHERE VIN = '{vehicle.VIN}'";
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

            #endregion

            #region Assert
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            #endregion
        }

        [Fact]
        public void ServiceLogCreation_InvalidServiceLogCategoryPassed_ServiceLogWrittenToSqlDB_Fail()
        {

        }

        [Fact]
        public void ServiceLogCreation_PartAttributeOverVarCharLimit_ServiceLogWrittenToSqlDb_Fail()
        {

        }

        [Fact]
        public void ServiceLogCreation_DateCreatedBeforeEarliestDateAllowed_ServiceLogWrittenToSqlDb_Fail()
        {

        }

        [Fact]
        public void ServiceLogCreation_DateCreatedInFuture_ServiceLogWrittenToSqlDb_Fail()
        {

        }

        [Fact]
        public void ServiceLogCreation_DescriptionAttributeOverVarCharLimit_ServiceLogWrittenToSqlDb_Fail()
        {

        }

        [Fact]
        public void ServiceLogCreation_MileageLessThan0_ServiceLogWrittenToSqlDb_Fail()
        {

        }

        [Fact]
        public void ServiceLogCreation_MileageGreaterThan_999_999_ServiceLogWrittenToSqlDb_Fail()
        {

        }
    }
}
