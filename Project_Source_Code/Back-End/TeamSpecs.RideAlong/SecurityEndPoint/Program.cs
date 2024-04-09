using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Middleware;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using Microsoft.Extensions.DependencyInjection;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
var builder = WebApplication.CreateBuilder(args);

// Using configuration files
var configuration = builder.Configuration;
// var myConfig = configuration.GetSection("").Get<>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IGenericDAO, SqlServerDAO> ();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IAuthTarget, SQLServerAuthTarget>();
builder.Services.AddScoped<ILogTarget, SqlDbLogTarget>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISecurityManager, SecurityManager>();


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
//app.useIDValidator();
app.UseHttpsRedirection();


// This is the last middleware, as we want to make sure it is not going to be overwritten at any point
app.useCorsMiddleware();

app.MapControllers();
app.Run();
