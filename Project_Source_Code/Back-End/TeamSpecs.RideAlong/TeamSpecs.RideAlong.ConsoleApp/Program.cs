using TeamSpecs.RideAlong;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.VehicleProfile;

var dao = new SqlServerDAO();
var vpt = new SqlDbVehicleTarget(dao);
var logt = new SqlDbLogTarget(dao);
var log = new LogService(logt);
var vprs = new VehicleProfileRetrievalService(vpt, log);


var account = new AccountUserModel(userName: "hello");
account.UserId = 7;
account.UserHash = "RAHHHH";
var temp = vprs.retrieveVehicleProfilesForUser(account);
foreach (var profile in temp.ReturnValue)
{
    var car = profile as VehicleProfileModel;
    Console.WriteLine(car.Make + " " + car.Model + " " + car.Year + " " + car.VIN + " " + car.LicensePlate);
}
