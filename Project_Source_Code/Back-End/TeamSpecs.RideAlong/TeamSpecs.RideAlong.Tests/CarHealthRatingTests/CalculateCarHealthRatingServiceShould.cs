using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.CarHealthRatingLibrary;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Model.ServiceLogModel;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.TestingLibrary.CarHealthRatingTests
{
    public class CalculateCarHealthRatingServiceShould
    {
        [Fact]
        public void CalculateCarHealthRating_ValidVINPassedIn_ZeroServiceLogsFound_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            long realUID;

            var dao = new SqlServerDAO();
            var chrTarget = new SqlDBCarHealthRatingTarget(dao);
            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(dao);
            var logService = new LogService(logTarget, hashService);
            var giveRankService = new CarHealthRatingService(chrTarget, logService);

            #region Creating Test objects
            var user = new AccountUserModel("testCarHealthRatingUser7")
            {
                Salt = 0,
                UserHash = "testCarHeathRatingUserHash7",
            };

            var vehicle = new VehicleProfileModel("testVin7", 1, "test", "testMake", "testModel", 2020);

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
            response = giveRankService.CalculateCarHealthRatingService(user, vehicle.VIN);
            timer.Stop();

            Thread.Sleep(1000); // Waiting for Log Async Task to finish

            #region Clean up DB
            var undoLogInsert = $"DELETE FROM Log WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoLogInsert, null)
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
            Assert.True(response.HasError);
            Assert.True(response.ErrorMessage == "User has no service logs on the vehicle " + vehicle.VIN);
            #endregion
        }

        [Fact]
        public void CalculateCarHealthRating_ValidVINPassedIn_ThreeServicelogsFound_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            long realUID;

            var dao = new SqlServerDAO();
            var chrTarget = new SqlDBCarHealthRatingTarget(dao);
            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(dao);
            var logService = new LogService(logTarget, hashService);
            var giveRankService = new CarHealthRatingService(chrTarget, logService);

            #region Creating Test objects
            var user = new AccountUserModel("testCarHealthRatingUser6")
            {
                Salt = 0,
                UserHash = "testCarHeathRatingUserHash6",
            };

            var vehicle = new VehicleProfileModel("testVin6", 1, "test", "testMake", "testModel", 2020);

            var serviceLogs = new List<ServiceLogModel>();
            for (int i = 0; i < 3; i++)
            {
                if (i % 2 == 0)
                {
                    var serviceLog = new ServiceLogModel("Maintenance", "Oil", DateTime.Now.AddDays(-i), $"Description {i}", i * 4000, vehicle.VIN);
                    serviceLogs.Add(serviceLog);
                }
                else
                {
                    var serviceLog = new ServiceLogModel("Maintenance", "Tires", DateTime.Now.AddDays(-i), $"Description {i}", i * 5000, vehicle.VIN);
                    serviceLogs.Add(serviceLog);
                }
            }
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

                foreach (var serviceLog in serviceLogs)
                {
                    var serviceLogSql = $@"
                    INSERT INTO ServiceLog (Category, Part, Date, Description, Mileage, VIN)
                    VALUES ('{serviceLog.Category}', '{serviceLog.Part}', '{serviceLog.Date:yyyy-MM-dd HH:mm:ss}', 
                    '{serviceLog.Description}', {serviceLog.Mileage}, '{serviceLog.VIN}')";

                    dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
                    {
                        KeyValuePair.Create<string, HashSet<SqlParameter>?>(serviceLogSql, null)
                    });
                }

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
            response = giveRankService.CalculateCarHealthRatingService(user, vehicle.VIN);
            timer.Stop();

            Thread.Sleep(1000); // Waiting for Log Async Task to finish

            #region Clean up DB
            var undoLogInsert = $"DELETE FROM Log WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoLogInsert, null)
            });

            var undoServiceLogInsert = $"DELETE FROM ServiceLog WHERE VIN = '{vehicle.VIN}'";
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
            Assert.True(response.HasError);
            Assert.True(response.ErrorMessage == "User has less than 10 service logs on the vehicle " + vehicle.VIN);
            #endregion
        }

        [Fact]
        public void CalculateCarHealthRating_ValidVINPassedIn_TenServiceLogsFound_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            long realUID;
            var pointsCheck = 0;
            var categoryCheck = 0;

            var dao = new SqlServerDAO();
            var chrTarget = new SqlDBCarHealthRatingTarget(dao);
            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(dao);
            var logService = new LogService(logTarget, hashService);
            var giveRankService = new CarHealthRatingService(chrTarget, logService);

            #region Creating Test objects
            var user = new AccountUserModel("testCarHealthRatingUser5")
            {
                Salt = 0,
                UserHash = "testCarHeathRatingUserHash5",
            };

            var vehicle = new VehicleProfileModel("testVin5", 1, "test", "testMake", "testModel", 2016);

            var serviceLogs = new List<ServiceLogModel>();
            for (int i = 0; i < 10; i++)
            {
                if (i == 3 || i == 4)
                {
                    var serviceLog = new ServiceLogModel("Maintenance", "Tire Pressure", DateTime.Now.AddDays(-i), $"Description {i}", i * 4000, vehicle.VIN);
                    serviceLogs.Add(serviceLog);
                }
                else if (i % 2 == 0)
                {
                    var serviceLog = new ServiceLogModel("Maintenance", "Oil", DateTime.Now.AddDays(-i), $"Description {i}", i * 4000, vehicle.VIN);
                    serviceLogs.Add(serviceLog);
                }
                else
                {
                    var serviceLog = new ServiceLogModel("Maintenance", "Tires", DateTime.Now.AddDays(-i), $"Description {i}", i * 5000, vehicle.VIN);
                    serviceLogs.Add(serviceLog);
                }
            }
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

                foreach (var serviceLog in serviceLogs)
                {
                    var serviceLogSql = $@"
                    INSERT INTO ServiceLog (Category, Part, Date, Description, Mileage, VIN)
                    VALUES ('{serviceLog.Category}', '{serviceLog.Part}', '{serviceLog.Date:yyyy-MM-dd HH:mm:ss}', 
                    '{serviceLog.Description}', {serviceLog.Mileage}, '{serviceLog.VIN}')";

                    dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
                    {
                        KeyValuePair.Create<string, HashSet<SqlParameter>?>(serviceLogSql, null)
                    });
                }

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
            response = giveRankService.CalculateCarHealthRatingService(user, vehicle.VIN);
            timer.Stop();

            Thread.Sleep(1000); // Waiting for Log Async Task to finish

            #region Clean up DB
            var undoLogInsert = $"DELETE FROM Log WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoLogInsert, null)
            });

            var undoServiceLogInsert = $"DELETE FROM ServiceLog WHERE VIN = '{vehicle.VIN}'";
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

            #region Check results
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            foreach (var resultObject in response.ReturnValue)
            {
                if (resultObject is IList list)
                {
                    foreach (var item in list)
                    {
                        if (item is string)
                        {
                            categoryCheck++;
                        }
                        else if (item is int)
                        {
                            pointsCheck++;
                        }
                    }
                }
                else if (resultObject is int) // Check if it's an integer
                {
                    pointsCheck++;
                }
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            #endregion

            #endregion

            #region Assert
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            Assert.NotNull(response.ReturnValue);
            Assert.True(response.ReturnValue.Count == 3); // Check for 3 objects
            // 7
            Assert.True(categoryCheck == 7);
            // 8 because add the totalPoints to Int Checker
            Assert.True(pointsCheck == 8);
            #endregion

        }

        // Edge case of when one item is in List and must be compared against most recent Service Log Mileage 
        [Fact]
        public void CaluclateCarHealthRating_ValidVINPassedIn_TenServiceLogsFound_OneItemInACategory_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();

            IResponse response;

            long realUID;
            var pointsCheck = 0;
            var categoryCheck = 0;

            var dao = new SqlServerDAO();
            var chrTarget = new SqlDBCarHealthRatingTarget(dao);
            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(dao);
            var logService = new LogService(logTarget, hashService);
            var giveRankService = new CarHealthRatingService(chrTarget, logService);

            #region Creating Test objects
            var user = new AccountUserModel("testCarHealthRatingUser4")
            {
                Salt = 0,
                UserHash = "testCarHeathRatingUserHash4",
            };

            var vehicle = new VehicleProfileModel("testVin4", 1, "test", "testMake", "testModel", 2016);

            var serviceLogs = new List<ServiceLogModel>();
            for (int i = 0; i < 10; i++)
            {
                if(i == 9)
                {
                    var serviceLog = new ServiceLogModel("Maintenance", "Tire Pressure", DateTime.Now.AddDays(-i), $"Description {i}", i * 1000, vehicle.VIN);
                    serviceLogs.Add(serviceLog);
                }
                else if (i % 2 == 0)
                {
                    var serviceLog = new ServiceLogModel("Maintenance", "Oil", DateTime.Now.AddDays(-i), $"Description {i}", i * 1000, vehicle.VIN);
                    serviceLogs.Add(serviceLog);
                }
                else
                {
                    var serviceLog = new ServiceLogModel("Maintenance", "Tires", DateTime.Now.AddDays(-i), $"Description {i}", i * 1000, vehicle.VIN);
                    serviceLogs.Add(serviceLog);
                }
            }
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

                foreach (var serviceLog in serviceLogs)
                {
                    var serviceLogSql = $@"
                    INSERT INTO ServiceLog (Category, Part, Date, Description, Mileage, VIN)
                    VALUES ('{serviceLog.Category}', '{serviceLog.Part}', '{serviceLog.Date:yyyy-MM-dd HH:mm:ss}', 
                    '{serviceLog.Description}', {serviceLog.Mileage}, '{serviceLog.VIN}')";

                    dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
                    {
                        KeyValuePair.Create<string, HashSet<SqlParameter>?>(serviceLogSql, null)
                    });
                }

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
            response = giveRankService.CalculateCarHealthRatingService(user, vehicle.VIN);
            timer.Stop();

            Thread.Sleep(1000); // Waiting for Log Async Task to finish

            #region Clean up DB
            var undoLogInsert = $"DELETE FROM Log WHERE UserHash = '{user.UserHash}'";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoLogInsert, null)
            });

            var undoServiceLogInsert = $"DELETE FROM ServiceLog WHERE VIN = '{vehicle.VIN}'";
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

            #region Check results
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            foreach (var resultObject in response.ReturnValue)
            {
                if (resultObject is IList list)
                {
                    foreach (var item in list)
                    {
                        if (item is string)
                        {
                            categoryCheck++;
                        }
                        else if (item is int)
                        {
                            pointsCheck++;
                        }
                    }
                }
                else if (resultObject is int) // Check if it's an integer
                {
                    pointsCheck++;
                }
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            #endregion

            #endregion

            #region Assert
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            Assert.NotNull(response.ReturnValue);
            Assert.True(response.ReturnValue.Count == 3); // Check for 3 objects
            // 8 because when there is one item, we have to do an additional comprasion with SL (1 item) to most recent SL to get socre
            Assert.True(categoryCheck == 8);
            // 9 because when there is one item, we have to do an additional comprasion with SL (1 item) to most recent SL to get score
            Assert.True(pointsCheck == 9);
            #endregion
        }
    }
}
