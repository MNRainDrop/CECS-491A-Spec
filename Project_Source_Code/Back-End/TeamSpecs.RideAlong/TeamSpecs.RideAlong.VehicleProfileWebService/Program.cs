var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use((context, next) =>
{
    // Handles Browser's preflight OPTIONS request
    if (context.Request.Method == nameof(HttpMethod.Options).ToUpperInvariant())
    {
        // use 204 cuz its a lighter payload
        context.Response.StatusCode = 204;

        // * should be the address - "http://localhost:3000/"
        context.Response.Headers.AccessControlAllowOrigin = "*";

        // allows the differnt types of calls
        // - Keep OPTIONS because the preflight needs it
        // - Only really need GET, POST, and DELETE
        context.Response.Headers.AccessControlAllowMethods = "GET, POST, OPTIONS, PUT, DELETE";

        // DONT USE STAR IN PRODUCTION
        // Determines what headers 
        // content view, content type, authorization, etag/expirations, vary, origin, etc.
        context.Response.Headers.AccessControlAllowHeaders = "*";

        // ENABLE THIS FOR AJAX
        context.Response.Headers.AccessControlAllowCredentials = "true";

        return Task.CompletedTask; // Terminate the HTTP Request
    }
    return next();
});

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
