
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure;
using TravelBooking.Application.Shared;
using Microsoft.IdentityModel.Tokens;
using TravelBooking.Infrastructure.Settings;
using System.Security.Claims;
using TravelBooking.Application.Interfaces;
using MediatR;
using TravelBooking.Api.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

// If using SQLite instead:
// options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))

// Add controllers, services, etc.
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            RoleClaimType = ClaimTypes.Role // VERY IMPORTANT
        };
    });

builder.Services.AddAuthorization();



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


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseSeeder.SeedAsync(db);
}

app.Run();
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
