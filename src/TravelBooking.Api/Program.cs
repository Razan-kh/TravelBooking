using TravelBooking.API.Extensions;
using TravelBooking.Infrastructure;
using TravelBooking.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration);

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateCityCommand>());

//builder.Services.AddControllers();
/*
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);        // API assembly
    cfg.RegisterServicesFromAssembly(typeof(GetRoomsQuery).Assembly);  // Application assembly
});
*/
/*
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TravelBooking.Infrastructure")
    ));
*/
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.ApplyMigrationsAndSeedAsync();

//app.UseHttpsRedirection();
app.MapControllers(); 
app.Run();