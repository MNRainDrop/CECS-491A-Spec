/*using TeamSpecs.RideAlong.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();


//CORS STUFF
//PREFLIGHT CONDITION HERE 

//app.useIDValidator();
app.useCorsPreflight();


//MIDDLEWARE RIGHT HERE

// Configure the HTTP request pipeline.
/*
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use((httpContent, next) =>
{
    httpContent.Response.Headers.AccessControlAllowOrigin = "*";
    httpContent.Response.Headers.AccessControlAllowMethods = "GET, POST, OPTIONS, PUT, DELETE";
    httpContent.Response.Headers.AccessControlAllowHeaders = "*";
    httpContent.Response.Headers.AccessControlAllowCredentials = "true";

    return next();
});


app.MapControllers();

app.Run();*/

using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.VehicleMarketplace;
using TeamSpecs.RideAlong.Middleware;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.VehicleMarketplace.Managers;
using TeamSpecs.RideAlong.ConfigService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddScoped<IConfigServiceJson, ConfigServiceJson>();
builder.Services.AddScoped<ISqlServerDAO, SqlServerDAO>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IAuthTarget, SQLServerAuthTarget>();
builder.Services.AddScoped<ILogTarget, SqlDbLogTarget>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISecurityManager, SecurityManager>();
builder.Services.AddScoped<IMarketplaceTarget, SqlDbMarketplaceTarget>();
builder.Services.AddScoped<IVehiceMarketplacePostCreationService, VehiceMarketplacePostCreationService>();
builder.Services.AddScoped<IVehiceMarketplacePostDeletionService, VehiceMarketplacePostDeletionService>();
builder.Services.AddScoped<IVehiceMarketplacePostRetrievalService, VehicleMarketplacePostRetrievalService>();
builder.Services.AddScoped<IVehicleMarketplaceRetrieveDetailVehicleProfileService, VehicleMarketplaceRetrieveDetailVehicleProfileService>();
builder.Services.AddScoped<IMailKitService, MailKitService>();
builder.Services.AddScoped<IVehiceMarketplaceSendBuyRequestService, VehiceMarketplaceSendBuyRequestService>();
builder.Services.AddScoped<IVehicleMarketplaceManager, VehicleMarketplaceManager>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.useCorsPreflight();

app.useIDValidator();

////

app.useCorsMiddleware();

app.MapControllers();

app.Run();
