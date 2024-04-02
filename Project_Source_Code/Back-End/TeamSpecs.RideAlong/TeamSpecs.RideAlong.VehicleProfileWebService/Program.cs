using TeamSpecs.RideAlong.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

app.useCorsPreflight();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.Use((httpContent, next) =>
{
    httpContent.Response.Headers.AccessControlAllowOrigin = "*";
    httpContent.Response.Headers.AccessControlAllowMethods = "GET, POST, OPTIONS, PUT, DELETE";
    httpContent.Response.Headers.AccessControlAllowHeaders = "*";
    httpContent.Response.Headers.AccessControlAllowCredentials = "true";

    return next();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
