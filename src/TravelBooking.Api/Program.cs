
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
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Application.Carts.Services.Implementations;
using TravelBooking.Application.Cheackout.Servicies.Implementations;
using TravelBooking.Application.ViewingHotels.Services.Implementations;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Infrastructure.Services.Email;
using TravelBooking.Application.ViewingHotels.Services.Interfaces;
using TravelBooking.Application.ViewingHotels.Mappers;
using TravelBooking.Application.Carts.Mappers;
using TravelBooking.API.Extensions;
using TravelBooking.Infrastructure;
using TravelBooking.Application;
using TravelBooking.Application.Images.Servicies;
using Serilog;
using Serilog.Sinks.Elasticsearch;

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

builder.Services.AddScoped<ImageAppService>();



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
    cfg.RegisterServicesFromAssemblies(typeof(HotelsController).Assembly,
                                       typeof(GetHotelDetailsQuery).Assembly);
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

if (!builder.Environment.IsEnvironment("Test"))
{
    // Normal SQL Server registration for production/dev
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    // In test environment, the DbContext will be replaced in ApiTestFactory
}

DotNetEnv.Env.Load();

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings")
);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);


var elasticUri = builder.Configuration["Elasticsearch:Uri"];

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "rooms-api-logs-{0:yyyy.MM.dd}"
    })
    .CreateLogger();

// Use Serilog
builder.Host.UseSerilog();


var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();  // ExceptionHandlingMiddleware MUST be first
app.UseMiddleware<RequestLoggingMiddleware>();



app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelBooking API V1");  //works on http://localhost:5063/index.html
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();

// Make the Program class public for WebApplicationFactory
public partial class Program { }
