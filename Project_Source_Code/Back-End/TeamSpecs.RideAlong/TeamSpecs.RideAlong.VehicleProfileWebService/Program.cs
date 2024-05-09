using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Middleware;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;
using TeamSpecs.RideAlong.ConfigService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IConfigServiceJson, ConfigServiceJson>();
builder.Services.AddScoped<IClaimTarget, ClaimTarget>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IGenericDAO, SqlServerDAO>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IAuthTarget, SQLServerAuthTarget>();
builder.Services.AddScoped<ILogTarget, SqlDbLogTarget>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISecurityManager, SecurityManager>();

builder.Services.AddScoped<ICRUDVehicleTarget, SqlDbVehicleTarget>();
builder.Services.AddScoped<IVehicleProfileRetrievalService, VehicleProfileRetrievalService>();
builder.Services.AddScoped<IVehicleProfileDetailsRetrievalService, VehicleProfileDetailsRetrievalService>();

builder.Services.AddScoped<IGetVehicleCountTarget, SqlDbVehicleTarget>();
builder.Services.AddScoped<IVehicleProfileCreationService, VehicleProfileCreationService>();
builder.Services.AddScoped<IVehicleProfileModificationService, VehicleProfileModificationService>();
builder.Services.AddScoped<IVehicleProfileDeletionService, VehicleProfileDeletionService>();

builder.Services.AddScoped<IVehicleProfileRetrievalManager, VehicleProfileRetrievalManager>();
builder.Services.AddScoped<IVehicleProfileCUDManager, VehicleProfileCUDManager>();

var app = builder.Build();

app.useCorsPreflight();

app.useIDValidator();

app.useCorsMiddleware();

app.MapControllers();

app.Run();
