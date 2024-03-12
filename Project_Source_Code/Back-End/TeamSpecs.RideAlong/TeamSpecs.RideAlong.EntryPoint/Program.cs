using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeamSpecs.RideAlong.Managers.Interfaces;
using TeamSpecs.RideAlong.Managers;
using TeamSpecs.RideAlong.UserAdministration;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.Services.HashService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IUserAdministrationManager, UserAdministrationManager>();
builder.Services.AddScoped<IAccountCreationService, AccountCreationService>();
builder.Services.AddScoped<IUserTarget, SqlDbUserTarget>();
builder.Services.AddScoped<IPepperTarget, FilePepperTarget>();
builder.Services.AddScoped<IJsonFileDAO, JsonFileDAO>();
builder.Services.AddScoped<ISqlServerDAO, SqlServerDAO>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IPepperService, PepperService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

// ensure index.html is loaded on start
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/index.html");
        return;
    }

    await next();
});

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Add("access-control-allow-credentials", "true");
        context.Response.Headers.Add("access-control-allow-headers", "content-type, authorization, ID");
        context.Response.Headers.Add("access-control-allow-methods", "GET, POST, OPTIONS");
        context.Response.Headers.Add("access-control-allow-origin", "*");
        context.Response.Headers.Add("access-control-max-age", "3600");
        var client = new Microsoft.Extensions.Primitives.StringValues();
        context.Request.Headers.TryGetValue("origin", out client);
        var method = context.Request.Method;
        if (method.Equals("OPTIONS"))
        {
            context.Response.StatusCode = 200;
        }
        context.Response.Headers.Remove("server");
        context.Response.Headers.Remove("X-Powered-By");
        return Task.FromResult(0);
    });
    await next(context);
});

app.MapControllers();

app.Run();
