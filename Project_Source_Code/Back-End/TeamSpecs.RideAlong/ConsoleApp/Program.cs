using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.SystemObservability;

var config = new ConfigServiceJson(); 
var dao = new SqlServerDAO(config);
var target = new SqlDbSystemObservabilityTarget(dao);

var logs = target.GetMostRegisteredVehiclesSql(1);
Console.Write(logs.ToString());
