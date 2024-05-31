using TeamSpecs.RideAlong.Archiving;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.Services.FormatCompressorService;
using TeamSpecs.RideAlong.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ISqlServerDAO, SqlServerDAO>();
builder.Services.AddScoped<IConfigServiceJson, ConfigServiceJson>();
builder.Services.AddScoped<IFormatCompressorService, JsonCompressorService>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<ILogTarget, SqlDbLogTarget>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IAuthTarget, SQLServerAuthTarget>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISecurityManager, SecurityManager>();
builder.Services.AddScoped<IArchivingTarget, SqlServerLogArchivingTarget>();
builder.Services.AddScoped<IArchivingService, ArchivingService>();
builder.Services.AddScoped<IReoccurringArchivingHostedService, MonthlyArchivingHostedService>();

var app = builder.Build();


app.useCorsPreflight();

app.useCorsMiddleware();

app.Run();
