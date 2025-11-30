using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Repositories;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Infrastructure.Services;
using TravelBooking.Application.Interfaces.Security;
using Sieve.Services;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Domain.Bookings.Repositories;
using TravelBooking.Domain.Reviews.Repositories;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Domain.Carts.Repositories;
using TravelBooking.Application.FeaturedDeals.Mappers;
using TravelBooking.Application.RecentlyVisited.Mappers;
using TravelBooking.Application.TrendingCities.Mappers;
using TravelBooking.Domain.Rooms.Interfaces;
using TravelBooking.Domain.Shared.Interfaces;
using TravelBooking.Infrastructure.Services.Images;
using TravelBooking.Domain.Images.interfaces;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Infrastructure.Services.Email;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; 
namespace TravelBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config, Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment)
    {
        // IOptions pattern for SMTP
        services.Configure<SmtpSettings>(options =>
        {
            options.Host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "localhost";
            options.Port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "25");
            options.Username = Environment.GetEnvironmentVariable("SMTP_USER") ?? "";
            options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
            options.EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("SMTP_ENABLESSL") ?? "false");
        });

        if (!environment.IsEnvironment("Test"))
        {
            // Normal SQL Server registration for production/dev
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        }
       else
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TestDatabase"));
        }

            // External service implementations
            services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IPdfService, QuestPdfService>();
        services.AddScoped<IPaymentService, MockPaymentService>();


        services.AddScoped<IAppDbContext>(provider =>
                  (IAppDbContext)provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

        services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ISieveProcessor, SieveProcessor>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        services.AddSingleton<IFeaturedHotelMapper, FeaturedHotelMapper>();
        services.AddSingleton<IRecentlyVisitedHotelMapper, RecentlyVisitedHotelMapper>();
        services.AddSingleton<ITrendingCityMapper, TrendingCityMapper>();

        // 1. Bind Cloudinary settings
        services.Configure<CloudinarySettings>(options =>
        {
            options.CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUDNAME") ?? string.Empty;
            options.ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_APIKEY") ?? string.Empty;
            options.ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_APISECRET") ?? string.Empty;
        });

        // 2. Register Cloudinary concrete service
        services.AddScoped<ICloudStorageService, CloudinaryService>();
        services.AddScoped<IGalleryImageRepository, GalleryImageRepository>(); // ← ADD THIS LINE

        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        return services;
    }
}

/*
namespace TravelBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
       // services.AddDbContext<AppDbContext>(options =>
           // options.UseSqlServer(config.GetConnectionString("DefaultConnection")));


services.AddScoped<IAppDbContext>(provider =>
            (IAppDbContext)provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

        services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ISieveProcessor, SieveProcessor>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        services.AddSingleton<IFeaturedHotelMapper, FeaturedHotelMapper>();
        services.AddSingleton<IRecentlyVisitedHotelMapper, RecentlyVisitedHotelMapper>();
        services.AddSingleton<ITrendingCityMapper, TrendingCityMapper>();


        // 1. Bind Cloudinary settings
        services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));

        // 2. Register Cloudinary concrete service
        services.AddScoped<ICloudStorageService, CloudinaryService>();
        services.AddScoped<IGalleryImageRepository, GalleryImageRepository>(); // ← ADD THIS LINE

        // Register other services
        return services;
    }
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        
     //   services.AddDbContext<AppDbContext>(options =>
    //    {
      //      options.UseSqlServer(
       //         configuration.GetConnectionString("DefaultConnection"),
       //         sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
      //  });
        
        
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<ICityRepository, CityRepository>();

        return services;
    }
}
*/