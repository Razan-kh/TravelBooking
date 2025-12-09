
using TravelBooking.Infrastructure;
using TravelBooking.API;
using TravelBooking.Application;
using Serilog;
using Serilog.Sinks.Elasticsearch;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);  
builder.Services.AddApplication();
builder.Services.AddApi(builder.Configuration);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var app = builder.Build();

app.UseApi();  

app.MapControllers();

app.Run();

public partial class Program { }