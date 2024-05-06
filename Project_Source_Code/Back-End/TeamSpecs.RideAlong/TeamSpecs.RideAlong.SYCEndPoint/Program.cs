using ScrapYourCarLibrary;
using ScrapYourCarLibrary.Interfaces;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.ScrapYourCarLibrary;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Interfaces;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IConfigServiceJson, ConfigServiceJson>();
builder.Services.AddScoped<IGenericDAO, SqlServerDAO>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IAuthTarget, SQLServerAuthTarget>();
builder.Services.AddScoped<ILogTarget, SqlDbLogTarget>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISecurityManager, SecurityManager>();
builder.Services.AddScoped<IBuyTarget, BuyTarget>();
builder.Services.AddScoped<IBuyRequestService, BuyRequestService>();
builder.Services.AddScoped<IPartsTarget, PartsTarget>();
builder.Services.AddScoped<IPartsService, PartsService>();
builder.Services.AddScoped<IListingSearchTarget, ListingSearchTarget>();
builder.Services.AddScoped<IListingSearchService, ListingSearchService>();
builder.Services.AddScoped<IScrapYourCarManager, ScrapYourCarManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
