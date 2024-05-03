using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.VehicleProfile;

var configService = new ConfigServiceJson();
var dao = new SqlServerDAO(configService);
var target = new SqlDbVehicleTarget(dao);

var user = new AccountUserModel("rainiermarlone@gmail.com")
{
    UserId = 0
};
var count = target.GetVehicleCount(user);
var value = count.ReturnValue.First<object>() as object[];

Console.WriteLine(value[0]);