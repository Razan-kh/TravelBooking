using TravelBooking.Infrastructure;
using TravelBooking.Application.ViewingHotels.Queries;
using TravelBooking.Infrastructure.Services;
using TravelBooking.Infrastructure.Persistence;
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

var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// in Infrastructure DI
builder.Services.AddInfrastructureServices();
builder.Services.AddControllers();

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
builder.Services.AddScoped<IHotelService, HotelService>();
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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();


app.Run();