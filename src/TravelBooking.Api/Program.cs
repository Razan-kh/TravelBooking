using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Infrastructure;
using TravelBooking.Infrastructure.Persistence.Seeders;
using TravelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Program.cs or Startup.cs

//builder.Services.AddDbContext<AppDbContext>(options => { /* ... */ });
/*
*/
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateCityCommand>());
// or AddMediatR(typeof(GetCitiesHandler).Assembly);

builder.Services.AddControllers();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);        // API assembly
    cfg.RegisterServicesFromAssembly(typeof(GetRoomsQuery).Assembly);  // Application assembly
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TravelBooking.Infrastructure")
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Run the Bogus seeder
    await BogusSeeder.SeedAsync(db);
}

app.UseHttpsRedirection();

app.Run();
