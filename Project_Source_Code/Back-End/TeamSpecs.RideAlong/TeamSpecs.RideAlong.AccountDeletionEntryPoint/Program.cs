using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration.Services;
using TeamSpecs.RideAlong.UserAdministration;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Managers;
using TeamSpecs.RideAlong.Middleware;
using Microsoft.AspNetCore.Components.Forms;
using TeamSpecs.RideAlong.UserAdministration.Targets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IConfigServiceJson, ConfigServiceJson>();
builder.Services.AddScoped<ISqlServerDAO, SqlServerDAO>();
builder.Services.AddScoped<IJsonFileDAO,JsonFileDAO>();

builder.Services.AddScoped<ISecurityManager, SecurityManager>();

builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IRandomService, RandomService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMailKitService, MailKitService>();

builder.Services.AddScoped<ILogTarget, SqlDbLogTarget>();

builder.Services.AddScoped<IAuthTarget, SQLServerAuthTarget>();
builder.Services.AddScoped<IClaimTarget, ClaimTarget>();

builder.Services.AddScoped<IAccountDeletionManager, AccountDeletionManager>();
builder.Services.AddScoped<IAccountDeletionService, AccountDeletionService>();
builder.Services.AddScoped<ISqlDbUserDeletionTarget, SqlDbUserDeletionTarget>();
builder.Services.AddScoped<IAccountRetrievalManager, AccountRetrievalManager>();
builder.Services.AddScoped<IAccountRetrievalService, AccountRetrievalService>();
builder.Services.AddScoped<ISqlDbUserRetrievalTarget, SqlDbUserRetrievalTarget>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// This is the first middleware, as we want it to exit as early as possible if we are handling a CORS Preflight
app.useCorsPreflight();

// Token validation is not necessary here, since if we are trying to log in, that means the user does not have tokens yet
app.useIDValidator();



// This is the last middleware, as we want to make sure it is not going to be overwritten at any point
app.useCorsMiddleware();

app.MapControllers();

app.Run();
