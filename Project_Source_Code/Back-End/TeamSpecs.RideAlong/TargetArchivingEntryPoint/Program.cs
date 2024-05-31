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
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IFormatCompressorService, JsonCompressorService>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IArchivingTarget>(serviceProvider =>
{
    var currentDirectory = Directory.GetCurrentDirectory();
    var fileName = "data.json";
    return new JsonArchivingTarget(currentDirectory, fileName, serviceProvider.GetRequiredService<IFormatCompressorService>());
});
builder.Services.AddScoped<IArchivingService, ArchivingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.useCorsPreflight();

app.useCorsMiddleware();

app.Run();
