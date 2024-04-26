// See https://aka.ms/new-console-template for more information
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.VehicleProfile;

Console.WriteLine("Hello, World!");

var dao = new SqlServerDAO();
var crud = new SqlDbVehicleTarget(dao);

var search = new List<object>()
{
    new KeyValuePair<string, string>("VIN", "NoOwner")
};
var vehicleInDB = crud.ReadVehicleProfileSql(search, 1, 1);

Console.WriteLine(vehicleInDB);