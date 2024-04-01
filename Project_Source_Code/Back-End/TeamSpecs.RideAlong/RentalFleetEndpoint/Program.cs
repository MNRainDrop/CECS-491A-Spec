using TeamSpecs.RideAlong.RentalFleetLibrary.Interfaces;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.RentalFleetLibrary;
using Microsoft.Extensions.DependencyInjection;
using TeamSpecs.RideAlong.Middleware;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IGenericDAO, SqlServerDAO>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<ILogTarget, SqlDbLogTarget>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IRentalFleetTarget, SqlServerRentalFleetTarget>();
builder.Services.AddScoped<IRentalFleetService, RentalFleetService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// app.useCorsPreflight();
app.Use((httpContext, next) =>
{
    // Preflight
    if (httpContext.Request.Method == nameof(HttpMethod.Options).ToUpperInvariant())
    {
        httpContext.Response.StatusCode = 204;      // This reduces the payload for a preflight check, will save a lot of money :P
        httpContext.Response.Headers.AccessControlAllowOrigin = "*"; // This should be dynamic
        //Typically we will scan all incoming requests, and find out if it belongs to an *allow list*
        httpContext.Response.Headers.AccessControlAllowMethods = "GET,POST,OPTIONS" + /*This is optional*/ ",PUT,DELETE";
        httpContext.Response.Headers.AccessControlAllowHeaders = "*"; // DO NOT USE STAR, Specify the headers we want them to have
        //Things such as content length, content type, authorization, expiration times, etc..... are all super important
        httpContext.Response.Headers.AccessControlAllowCredentials = "true"; // This is if we are going to use AJAX, since we need to send cookies

        return Task.CompletedTask; // Terminates the HTTP Request
    }
    return next(); // If this is not an options request, we should skip 
});

//app.UseAuthentication();

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.useCorsMiddleware();
app.Use((httpContext, next) =>
{
    httpContext.Response.Headers.AccessControlAllowOrigin = "*";
    httpContext.Response.Headers.AccessControlAllowMethods = "GET,POST,OPTIONS" + /*This is optional*/ ",PUT,DELETE";
    httpContext.Response.Headers.AccessControlAllowHeaders = "*";
    httpContext.Response.Headers.AccessControlAllowCredentials = "true";

    return next();
});

app.MapControllers();

app.Run();
