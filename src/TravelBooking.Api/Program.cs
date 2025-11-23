
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure;
using TravelBooking.Application.Shared;
using Microsoft.IdentityModel.Tokens;
using TravelBooking.Infrastructure.Settings;
using System.Security.Claims;
using MediatR;
using TravelBooking.Api.Controllers;
using TravelBooking.Application.ViewingHotels.Queries;
using TravelBooking.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.AddingToCart.Services.Interfaces;
using TravelBooking.Application.AddingToCart.Services.Implementations;
using TravelBooking.Application.Cheackout.Servicies.Implementations;
using TravelBooking.Application.ViewingHotels.Services.Implementations;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Infrastructure.Services.Email;
using TravelBooking.Application.ViewingHotels.Services.Interfaces;
using TravelBooking.Application.ViewingHotels.Mappers;
using TravelBooking.Application.AddingToCart.Mappers;
using TravelBooking.API.Extensions;
using TravelBooking.Infrastructure;
using TravelBooking.Application;

var builder = WebApplication.CreateBuilder(args);


// Add builder.Services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddApplicationServices();

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

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<GetHotelDetailsQuery>();
});

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddSingleton<IHotelMapper, HotelMapper>();
builder.Services.AddSingleton<IRoomCategoryMapper, RoomCategoryMapper>();
builder.Services.AddSingleton<IGalleryImageMapper, GalleryImageMapper>();
builder.Services.AddSingleton<IReviewMapper, ReviewMapper>();


builder.Services.AddScoped<IPaymentService, MockPaymentService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IPdfService, QuestPdfService>();

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<TravelBooking.Application.Searching.Servicies.Interfaces.IHotelService, TravelBooking.Application.Searching.Servicies.Implementations.HotelService>();
builder.Services.AddScoped<TravelBooking.Application.ViewingHotels.Services.Interfaces.IHotelService, TravelBooking.Application.ViewingHotels.Services.Implementations.HotelService>();

builder.Services.AddScoped<IRoomAvailabilityService, RoomAvailabilityService>();

builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICartMapper, CartMapper>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString); // or UseNpgsql, UseSqlite, etc.
});

DotNetEnv.Env.Load();

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings")
);

builder.Services.AddInfrastructureServices(builder.Configuration);
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

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

/*
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseSeeder.SeedAsync(db);
}
*/
app.Run();
app.UseRouting();
app.UseAuthorization(); // optional if using auth

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers(); // only call once

// Seed database\
/*
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ISeedService>();
    await seeder.SeedAsync();
}
*/
app.Run();
