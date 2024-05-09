using TeamSpecs.RideAlong.Middleware;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.SystemObservability;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IConfigServiceJson, ConfigServiceJson>();
builder.Services.AddScoped<IGenericDAO, SqlServerDAO>();
builder.Services.AddScoped<ISystemObservabilityTarget, SqlDbSystemObservabilityTarget>();


var app = builder.Build();

app.useCorsPreflight();

app.useIDValidator();

app.useCorsMiddleware();

app.MapControllers();

app.Run();
