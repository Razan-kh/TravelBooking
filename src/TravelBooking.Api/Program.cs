
using TravelBooking.Infrastructure;
using TravelBooking.API;
using TravelBooking.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);  
builder.Services.AddApplication();
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

app.UseApi();  // Exception middleware, logging, swagger, endpoints

app.MapControllers();

app.Run();

public partial class Program { }