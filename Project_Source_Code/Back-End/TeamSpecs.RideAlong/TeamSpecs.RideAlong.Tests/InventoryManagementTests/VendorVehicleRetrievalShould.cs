using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.InventoryManagement;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;


namespace TeamSpecs.RideAlong.TestingLibrary.InventoryManagementTests;

public class VendorVehicleRetrievalShould
{
    private static readonly IConfigServiceJson configService = new ConfigServiceJson();
    private static readonly IGenericDAO dao = new SqlServerDAO(configService);
    private static readonly HashService hash = new HashService();
    private static readonly ILogTarget logt = new SqlDbLogTarget(dao);
    private static readonly ILogService log = new LogService(logt, hash);
    private static readonly IRetrieveVendorVehicleTarget veh = new SqlDbVendorVehicleTarget(dao);
    private static readonly IRetrieveVendorVehicleService service = new RetrieveVendorVehicleService(veh, log);

    private long writeUserToDB(IGenericDAO dao, IAccountUserModel user)
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
        }
        return user.UserId;
    }

    private IAccountUserModel createTestUser()
    {
        // Create Test Objects
        return new AccountUserModel("testUser")
        {
            Salt = 0,
            UserHash = "testUserHash",
        };
    }

    private void writeVehicleToDB(IGenericDAO dao, IVendorVehicleModel vehicle)
    {
        var vehicleprofilesql = $"INSERT INTO VehicleProfile (VIN, Owner_UID, LicensePlate, Make, Model, Year) VALUES ('{vehicle.VIN}', {vehicle.Owner_UID}, '{vehicle.LicensePlate}', '{vehicle.Make}', '{vehicle.Model}', {vehicle.Year})";
        var vehicledetailssql = $"INSERT INTO VehicleDetails (VIN, Color, Description) VALUES ('{vehicle.VIN}', '{vehicle.Color}', '')";
        var vendingstatussql = $"INSERT INTO VendingStatus (VIN, Status, Price, Inquiries) VALUES ('{vehicle.VIN}', '{vehicle.Status}', {vehicle.Price}, {vehicle.Inquiries})";
        dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicleprofilesql, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(vehicledetailssql, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(vendingstatussql, null)
        });
    }
    private IVendorVehicleModel createTestVehicle(long uid, int iteration = 0)
    {
        return new VendorVehicleModel($"vin{iteration}", uid, "TESTING")
        {
            Make = "TESTING",
            Model = "TESTING",
            Year = 0,
            Status = "TESTING",
            PostingDate = DateTime.UnixEpoch,
            Price = 0,
            PriceDate = DateTime.UnixEpoch,
            Inquiries = 0,
            Color = "TEST"
        };
    }

    private void deleteFromDB(IGenericDAO dao, IAccountUserModel user)
    {
        var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'";
        dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
        });
    }


    [Fact]
    public void VendorVehicleRetrieval_ReadVendorVehicleProfilesFromDatabase_ValidUserAccountAndNoFiltersPassedIn_OneVehicleProfileRetrieved()
    {
        #region Arrange
        IResponse response;
        var timer = new Stopwatch();


        var user = createTestUser();
        var page = 1;
        var itemsPerPage = 10;
        var resultNum = 1;

        try
        {
            user.UserId = writeUserToDB(dao, user);
        }
        catch
        {
            deleteFromDB(dao, user);
        }
        var vehicle = createTestVehicle(user.UserId);

        try
        {
            
            writeVehicleToDB(dao, vehicle);
        }
        catch
        {
            deleteFromDB(dao, user);
        }
        
        #endregion

        #region Act
        try
        {
            timer.Start();
            response = service.retrieveVendorVehicles(user, page, itemsPerPage);
            timer.Stop();
        }
        finally
        {
            deleteFromDB(dao, user);
        }
        #endregion

        #region Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.NotNull(response);
        Assert.True(!response.HasError);
        Assert.NotNull(response.ReturnValue);
        Assert.True(response.ReturnValue.Count <= itemsPerPage);
        Assert.True(response.ReturnValue.Count == resultNum);
        var returnedVehicle = response.ReturnValue.FirstOrDefault() as VendorVehicleModel;
        Assert.NotNull(returnedVehicle);
        Assert.True(returnedVehicle.VIN == vehicle.VIN);
        Assert.True(returnedVehicle.Owner_UID == vehicle.Owner_UID);
        Assert.True(returnedVehicle.LicensePlate == vehicle.LicensePlate);
        Assert.True(returnedVehicle.Make == vehicle.Make);
        Assert.True(returnedVehicle.Model == vehicle.Model);
        Assert.True(returnedVehicle.Year == vehicle.Year);
        Assert.True(returnedVehicle.Status == vehicle.Status);
        Assert.True(returnedVehicle.Price == vehicle.Price);
        Assert.True(returnedVehicle.Inquiries == vehicle.Inquiries);
        Assert.True(returnedVehicle.Color == vehicle.Color);
        #endregion
    }
    [Fact]
    public void VendorVehicleRetrieval_ReadVendorVehicleProfilesFromDatabase_ValidUserAccountAndFiltersPassedIn_OneVehicleProfileRetrieved()
    {
        #region Arrange
        IResponse response;
        var timer = new Stopwatch();

        var user = createTestUser();
        var page = 1;
        var itemsPerPage = 10;
        var resultNum = 1;

        try
        {
            user.UserId = writeUserToDB(dao, user);
        }
        catch
        {
            deleteFromDB(dao, user);
        }
        var vehicle = createTestVehicle(user.UserId);
        vehicle.Make = "Acura";
        vehicle.VIN = "TESTACURA";
        var vehicle2 = createTestVehicle(user.UserId);
        try
        {
            writeVehicleToDB(dao, vehicle);
            writeVehicleToDB(dao, vehicle2);
        }
        catch
        {
            deleteFromDB(dao, user);
        }

        var searchFilters = new List<object>()
        {
            new KeyValuePair<string, string>("Make", "Acura"),
            new KeyValuePair<string, DateTime[]>("PostingDate", [DateTime.UnixEpoch, DateTime.UtcNow])
        };
        #endregion

        #region Act
        try
        {
            response = service.retrieveVendorVehicles(user, page, itemsPerPage, searchFilters);
        }
        finally
        {
            deleteFromDB(dao, user);
        }
        #endregion

        #region Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.NotNull(response);
        Assert.True(!response.HasError);
        Assert.NotNull(response.ReturnValue);
        Assert.True(response.ReturnValue.Count <= itemsPerPage);
        Assert.True(response.ReturnValue.Count == resultNum);
        var returnedVehicle = response.ReturnValue.FirstOrDefault() as VendorVehicleModel;
        Assert.NotNull(returnedVehicle);
        Assert.True(returnedVehicle.VIN == vehicle.VIN);
        Assert.True(returnedVehicle.Owner_UID == vehicle.Owner_UID);
        Assert.True(returnedVehicle.LicensePlate == vehicle.LicensePlate);
        Assert.True(returnedVehicle.Make == vehicle.Make);
        Assert.True(returnedVehicle.Model == vehicle.Model);
        Assert.True(returnedVehicle.Year == vehicle.Year);
        Assert.True(returnedVehicle.Status == vehicle.Status);
        Assert.True(returnedVehicle.Price == vehicle.Price);
        Assert.True(returnedVehicle.Inquiries == vehicle.Inquiries);
        Assert.True(returnedVehicle.Color == vehicle.Color);
        #endregion
    }
}