using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Middleware;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.VehicleProfile;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IGenericDAO, SqlServerDAO>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IAuthTarget, SQLServerAuthTarget>();
builder.Services.AddScoped<ILogTarget, SqlDbLogTarget>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISecurityManager, SecurityManager>();
builder.Services.AddScoped<IRetrieveVehicleDetailsTarget, SqlDbVehicleTarget>();
builder.Services.AddScoped<IRetrieveVehiclesTarget, SqlDbVehicleTarget>();
builder.Services.AddScoped<IVehicleProfileRetrievalService, VehicleProfileRetrievalService>();
builder.Services.AddScoped<IVehicleProfileDetailsRetrievalService, VehicleProfileDetailsRetrievalService>();
builder.Services.AddScoped<IVehicleProfileRetrievalManager, VehicleProfileRetrievalManager>();

builder.Services.AddScoped<ICreateVehicleTarget, SqlDbVehicleTarget>();
builder.Services.AddScoped<IModifyVehicleTarget, SqlDbVehicleTarget>();
builder.Services.AddScoped<IDeleteVehicleTarget, SqlDbVehicleTarget>();
builder.Services.AddScoped<IVehicleProfileCreationService, VehicleProfileCreationService>();
builder.Services.AddScoped<IVehicleProfileModificationService, VehicleProfileModificationService>();
builder.Services.AddScoped<IVehicleProfileDeletionService, VehicleProfileDeletionService>();
builder.Services.AddScoped<IVehicleProfileCUDManager, VehicleProfileCUDManager>();

var app = builder.Build();

//// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.useCorsPreflight();

app.useIDValidator();

app.useCorsMiddleware();

app.MapControllers();

app.Run();
