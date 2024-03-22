using TeamSpecs.RideAlong.Middleware;
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

// This is the first middleware, as we want it to exit as early as possible if we are handling a CORS Preflight
app.useCorsPreflight();

app.UseHttpsRedirection();

app.UseAuthorization();

// This is the last middleware, as we want to make sure it is not going to be overwritten at any point
app.useCorsMiddleware();

app.MapControllers();

app.Run();
