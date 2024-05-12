using TeamSpecs.RideAlong.Middleware;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.SystemObservability;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IConfigServiceJson, ConfigServiceJson>();
builder.Services.AddScoped<ISqlServerDAO, SqlServerDAO>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IAuthTarget, SQLServerAuthTarget>();
builder.Services.AddScoped<ILogTarget, SqlDbLogTarget>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISecurityManager, SecurityManager>();

builder.Services.AddScoped<ISystemObservabilityTarget, SqlDbSystemObservabilityTarget>();
builder.Services.AddScoped<ISystemObservabilityService, SystemObservabilityService>();
builder.Services.AddScoped<ISystemObservabilityManager, SystemObservabilityManager>();


var app = builder.Build();

app.useCorsPreflight();

app.useIDValidator();

app.useCorsMiddleware();

app.MapControllers();

app.Run();
