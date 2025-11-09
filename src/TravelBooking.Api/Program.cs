using TravelBooking.Infrastructure;
using TravelBooking.Application.Interfaces;
using MediatR;
using System.Reflection;
using TravelBooking.Application.Handlers;
using TravelBooking.Application.Utils;
using TravelBooking.Api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssemblyContaining<HotelsController>();
});

builder.Services.AddApplication();
var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization(); // optional if using auth

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers(); // only call once

// Seed database
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ISeedService>();
    await seeder.SeedAsync();
}

app.Run();
