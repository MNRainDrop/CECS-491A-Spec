using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;

var dao = new SqlServerDAO();
var vpt = new SqlDbVehicleTarget(dao);
var logt = new SqlDbLogTarget(dao);
var hs = new HashService();
var log = new LogService(logt, hs);
var vprs = new VehicleProfileRetrievalService(vpt, log);


var account = new AccountUserModel(userName: "hello");
account.UserId = 7;
account.UserHash = "RAHHHH";
var temp = vprs.retrieveVehicleProfilesForUser(account);
foreach (IVehicleProfileModel car in temp.ReturnValue)
{
    Console.WriteLine(car.Make + " " + car.Model + " " + car.Year + " " + car.VIN + " " + car.LicensePlate);
}
